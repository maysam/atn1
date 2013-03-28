using System;
using System.Collections.Generic;
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
            TheoryMembership tm = Context.TheoryMemberships.Where(t => t.TheoryId == TheoryId && t.SourceId == SourceId).OrderBy(t => t.RunId).FirstOrDefault();
            TheoryMembershipSignificance tms = Context.TheoryMembershipSignificances.Single(t => t.SourceId == SourceId && t.TheoryId == TheoryId);
            int NumberContributing = Context.MetaAnalysisMemberships.Where(mam => mam.TheoryMembershipSignificanceId == tms.TheoryMembershipSignificanceId).Count();
            return new CompleteTheoryMembership(tm, tms, NumberContributing);
        }
        public void InitiateTheoryAnalysis(int TheoryId, bool StoreImpactFactor)
        {
            //Add new analysis run
            Run r = new Run();
            r.DateStarted = DateTime.Now;
            r.TheoryId = TheoryId;
            Context.Runs.AddObject(r);
            Context.SaveChanges();

            Theories t = new Theories(Context);
            SourceWithDepth[] AllLevelSources = t.GetAllSourcesForTheory(TheoryId);
            foreach (SourceWithDepth Source in AllLevelSources)
            {
                TheoryMembershipSignificance tms = GetTheoryMembershipSignificanceForSource(Source.Source.SourceId, TheoryId);
                if (tms == null)
                {
                    tms = new TheoryMembershipSignificance();
                    tms.SourceId = Source.Source.SourceId;
                    tms.TheoryId = TheoryId;
                    tms.RAMarkedContributing = null;
                    tms.IsMetaAnalysis = false;
                    Context.TheoryMembershipSignificances.AddObject(tms);
                    Context.SaveChanges();
                }
                TheoryMembership tm = new TheoryMembership();
                tm.TheoryId = TheoryId;
                tm.SourceId = Source.Source.SourceId;
                tm.RunId = r.RunId;
                tm.Depth = Source.Depth;
                if (StoreImpactFactor)
                {
                    tm.ImpactFactor = Source.ImpactFactor;
                }
                Context.TheoryMemberships.AddObject(tm);
            }
            Context.SaveChanges();
        }
    }
}
