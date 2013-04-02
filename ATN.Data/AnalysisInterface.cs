using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ATN.Data
{
    public class AnalysisInterface: DatabaseInterface
    {
        public AnalysisInterface(ATNEntities Entities = null) : base(Entities)
        {

        }
        /// <summary>
        /// Retrieves the most recent TheoryMembership object for the specified source.
        /// </summary>
        /// <param name="SourceId">Source for which to retrieve the most recent TheoryMembership</param>
        /// <returns>Most recent TheoryMembership</returns>
        public TheoryMembership GetTheoryMembershipForSource(long SourceId, int TheoryId)
        {
            return Context.TheoryMemberships.Where(
                tm => tm.SourceId == SourceId &&
                    tm.TheoryId == TheoryId
                ).OrderByDescending(
                tm => tm.RunId
                ).FirstOrDefault();
        }

        public TheoryMembershipSignificance GetTheoryMembershipSignificanceForSource(long SourceId, int TheoryId)
        {
            return Context.TheoryMembershipSignificances.Where(
                tms => tms.SourceId == SourceId &&
                    tms.TheoryId == TheoryId
                ).FirstOrDefault();
        }
        public void UpdateTheoryAttentionRatio(int TheoryId, long SourceId, double TAR)
        {
            TheoryMembership TM = GetTheoryMembershipForSource(SourceId, TheoryId);
            TM.TheoryAttributionRatio = TAR;
            Context.SaveChanges();
        }

        public void UpdateImpactFactor(int TheoryId, long SourceId)
        {
            TheoryMembership TM = GetTheoryMembershipForSource(SourceId, TheoryId);
            TM.ImpactFactor = Context.Sources.Single(s => s.SourceId == SourceId).CitingSources.Count();
            Context.SaveChanges();
        }
        /// <summary>
        /// Retrieves all papers marked by hand as either contributing or not.
        /// Used in the construction of ML training data. Meta Anlyses are
        /// excluded from results array.
        /// </summary>
        /// <param name="TheoryId">Theory for which to retrieve marked sources.</param>
        /// <returns>All marked sources for a particular theory.</returns>
        public Source[] GetMarkedSourcesForTheory(int TheoryId)
        {
            return Context.TheoryMembershipSignificances.Where(
                s => s.TheoryId == TheoryId &&
                    s.RAMarkedContributing.HasValue &&
                    s.IsMetaAnalysis == false
                    ).Join(
                Context.Sources, y => y.SourceId, x => x.SourceId, (u, s) => s
                ).ToArray();
        }

        public Source[] GetUnmarkedSourcesForTheory(int TheoryId)
        {
            return Context.TheoryMembershipSignificances.Where(
                s => s.TheoryId == TheoryId &&
                    s.RAMarkedContributing.HasValue == false &&
                    s.IsMetaAnalysis == false
                    ).Join(
                Context.Sources, y => y.SourceId, x => x.SourceId, (u, s) => s
                ).ToArray();
        }
        public CompleteTheoryMembership GetTheoryMembershipContributionsForSource(int TheoryId, long SourceId)
        {
            TheoryMembership tm = Context.TheoryMemberships.Where(t => t.TheoryId == TheoryId && t.SourceId == SourceId).OrderByDescending(t => t.RunId).FirstOrDefault();
            TheoryMembershipSignificance tms = Context.TheoryMembershipSignificances.Single(t => t.SourceId == SourceId && t.TheoryId == TheoryId);
            int NumberContributing = Context.MetaAnalysisMemberships.Where(mam => mam.TheoryMembershipSignificanceId == tms.TheoryMembershipSignificanceId).Count();
            return new CompleteTheoryMembership(tm, tms, NumberContributing);
        }

        public void UpdateAEFScore(int TheoryId, int RunId, Dictionary<long, double> SourceIDAEFScores)
        {
            SourceIDAEFScores = SourceIDAEFScores.Where(kv => kv.Value != 0).ToDictionary(kv => kv.Key, kv => kv.Value);
            StringBuilder QueryBuilder = new StringBuilder();
            QueryBuilder.AppendLine("CREATE TABLE #SourceIdTable (SourceId bigint PRIMARY KEY, AEF float);");
            for (int i = 0; i < SourceIDAEFScores.Count / 1000 + 1; i++)
            {
                if (SourceIDAEFScores.Count - 1000 * i > 0)
                {
                    QueryBuilder.AppendLine("INSERT INTO #SourceIdTable VALUES " + String.Join(",", SourceIDAEFScores.Skip(i * 1000).Take(1000).Select(s => "(" + s.Key + "," + s.Value + ")").ToArray()) + ";");
                }
            }
            QueryBuilder.AppendLine(string.Format("UPDATE TheoryMembership SET ArticleLevelEigenfactor = 0 WHERE TheoryId = {0} AND RunID = {1}", TheoryId, RunId));
            QueryBuilder.AppendLine(string.Format("UPDATE tm SET tm.ArticleLevelEigenfactor = st.AEF FROM TheoryMembership AS tm INNER JOIN #SourceIdTable AS st ON tm.SourceId = st.SourceId WHERE tm.TheoryId = {0} AND tm.RunId = {1}", TheoryId, RunId));
            QueryBuilder.AppendLine("DROP TABLE #SourceIdTable;");
            Context.CommandTimeout = 240;
            Context.ExecuteStoreCommand(QueryBuilder.ToString());
        }

        public int InitiateTheoryAnalysis(int TheoryId, bool StoreImpactFactor, SourceWithReferences[] AllLevelSources)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            //Add new analysis run
            Run r = new Run();
            r.DateStarted = DateTime.Now;
            r.TheoryId = TheoryId;
            Context.Runs.AddObject(r);
            Context.SaveChanges();

            Theories t = new Theories(Context);
            var ExistingTheoryMembershiSignificance = AllLevelSources.Join(Context.TheoryMembershipSignificances, al => new { al.SourceId, TheoryId }, tm => new { tm.SourceId, tm.TheoryId }, (al, tm) => tm.SourceId).ToList();
            SourceWithReferences[] SourcesNeedingTheoryMembershipSignificances = AllLevelSources.Where(als => !ExistingTheoryMembershiSignificance.Contains(als.SourceId)).ToArray();
            if (SourcesNeedingTheoryMembershipSignificances.Length > 0)
            {
                StringBuilder TheoryMembershipSignificanceQueryBuilder = new StringBuilder();
                for (int i = 0; i < SourcesNeedingTheoryMembershipSignificances.Length / 1000 + 1; i++)
                {
                    //This is to prevent any number of needed rows throwing an error when it is
                    //perfectly divisible by 1000, as the resulting query would be of the form
                    //INSERT ... VALUES; with no included values
                    if (SourcesNeedingTheoryMembershipSignificances.Length - 1000 * i > 0)
                    {
                        TheoryMembershipSignificanceQueryBuilder.AppendLine("INSERT INTO TheoryMembershipSignificance (TheoryId, SourceId, RAMarkedContributing, IsMetaAnalysis) VALUES " + String.Join(",", SourcesNeedingTheoryMembershipSignificances.Skip(i * 1000).Take(1000).Select(s => "(" + TheoryId + "," + s.SourceId + ",NULL,0)").ToArray()));
                    }
                }
                Context.CommandTimeout = 120;
                Context.ExecuteStoreCommand(TheoryMembershipSignificanceQueryBuilder.ToString());
            }

            StringBuilder QueryBuilder = new StringBuilder();
            for (int i = 0; i < AllLevelSources.Length / 1000 + 1; i++)
            {
                //This is to prevent any number of needed rows throwing an error when it is
                //perfectly divisible by 1000, as the resulting query would be of the form
                //INSERT ... VALUES; with no included values
                if (AllLevelSources.Length - 1000 * i > 0)
                {
                    QueryBuilder.AppendLine("INSERT INTO TheoryMembership (TheoryId, SourceId, RunId, Depth, ImpactFactor) VALUES " + String.Join(",", AllLevelSources.Skip(i * 1000).Take(1000).Select(s => "(" + TheoryId + "," + s.SourceId + "," + r.RunId + "," + s.Depth + "," + (StoreImpactFactor ? s.ImpactFactor.ToString() : "NULL") + ")").ToArray()) + ";");
                }
            }
            Context.CommandTimeout = 120;
            Context.ExecuteStoreCommand(QueryBuilder.ToString());

            sw.Stop();
            Trace.WriteLine(string.Format("Theory initialization: {0}", sw.Elapsed));
            return r.RunId;
        }
    }
}
