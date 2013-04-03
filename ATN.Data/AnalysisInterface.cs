using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.Samples.EntityDataReader;

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

        /// <summary>
        /// Returns a complete representation of theory membership contributing for a given theory and member source
        /// </summary>
        /// <param name="TheoryId">The theory from which to retrieve a source's contributing</param>
        /// <param name="SourceId">The source to retrieve contribution</param>
        /// <returns>A complete representation of a source's contribution to a given theory</returns>
        public CompleteTheoryMembership GetTheoryMembershipContributionsForSource(int TheoryId, long SourceId)
        {
            TheoryMembership tm = Context.TheoryMemberships.Where(t => t.TheoryId == TheoryId && t.SourceId == SourceId).OrderByDescending(t => t.RunId).FirstOrDefault();
            TheoryMembershipSignificance tms = Context.TheoryMembershipSignificances.Single(t => t.SourceId == SourceId && t.TheoryId == TheoryId);
            int NumberContributing = Context.MetaAnalysisMemberships.Where(mam => mam.TheoryMembershipSignificanceId == tms.TheoryMembershipSignificanceId).Count();
            return new CompleteTheoryMembership(tm, tms, NumberContributing);
        }

        /// <summary>
        /// Creates all of the neccessary database records to being performing theory anaysis
        /// </summary>
        /// <param name="TheoryId">The theory to initiate analysis of</param>
        /// <param name="StoreImpactFactor">Whether to store the ImpactFactor of the theory's member sources</param>
        /// <param name="AllLevelSources">All sources which are members of the theory</param>
        /// <returns>The RunId of the analysis run being initiated</returns>
        public int InitializeTheoryAnalysis(int TheoryId, SourceWithReferences[] AllLevelSources)
        {
            //Add new analysis run
            Run r = new Run();
            r.DateStarted = DateTime.Now;
            r.TheoryId = TheoryId;
            Context.Runs.AddObject(r);
            Context.SaveChanges();

            Theories t = new Theories(Context);

            long[] SourceIdsWithTms = Context.ExecuteStoreQuery<long>(
                @"CREATE TABLE #InitiateTheoryTable (SourceId bigint, CitesSourceId bigint NULL, Depth SMALLINT);
                CREATE CLUSTERED INDEX Index_SourceIdTable_SourceId ON #InitiateTheoryTable(SourceId, CitesSourceId)
                INSERT INTO #InitiateTheoryTable SELECT s.SourceId as SourceId, NULL, 0 as Depth FROM Source s WHERE SourceId IN (" + String.Join(",", AllLevelSources.TakeWhile(als => als.Depth == 0).Select(cs => cs.SourceId).ToArray()) + @");
                INSERT INTO #InitiateTheoryTable SELECT c.SourceId as SourceId, st.SourceId, 1 as Depth FROM CitationsReference c JOIN #InitiateTheoryTable st ON st.SourceId = c.CitesSourceId WHERE st.Depth = 0;
                INSERT INTO #InitiateTheoryTable SELECT st.SourceId as SourceId, c.CitesSourceId as CitesSourceId, 2 as Depth FROM CitationsReference c JOIN #InitiateTheoryTable st ON st.SourceId = c.SourceId WHERE st.Depth = 1;
                SELECT DISTINCT st1.SourceId FROM #InitiateTheoryTable st1 WHERE st1.SourceId NOT IN (SELECT DISTINCT SourceId FROM TheoryMembershipSignificance tms WHERE tms.TheoryId = {0}) UNION SELECT DISTINCT CitesSourceId FROM #InitiateTheoryTable st2 WHERE st2.CitesSourceId IS NOT NULL AND st2.CitesSourceId NOT IN (SELECT DISTINCT SourceId FROM TheoryMembershipSignificance tms WHERE tms.TheoryId = {0});
                DROP INDEX Index_SourceIdTable_SourceId ON #InitiateTheoryTable;
                DROP TABLE #InitiateTheoryTable;",
                TheoryId).ToArray();
            //var ExistingTheoryMembershiSignificance = AllLevelSources.Join(Context.TheoryMembershipSignificances, al => new { al.SourceId, TheoryId }, tm => new { tm.SourceId, tm.TheoryId }, (al, tm) => tm.SourceId).ToList();
            SourceWithReferences[] SourcesNeedingTheoryMembershipSignificances = SourceIdsWithTms.Distinct().Join(AllLevelSources, sitms => sitms, als => als.SourceId, (sitms, als) => als).ToArray();

            if (SourcesNeedingTheoryMembershipSignificances.Length > 0)
            {
                SqlBulkCopy TheoryMembershipSignificanceCopier = new SqlBulkCopy(((EntityConnection)Context.Connection).StoreConnection.ConnectionString, SqlBulkCopyOptions.KeepIdentity);
                TheoryMembershipSignificanceCopier.DestinationTableName = "TheoryMembershipSignificance";
                TheoryMembershipSignificanceCopier.ColumnMappings.Add(0, 1);
                TheoryMembershipSignificanceCopier.ColumnMappings.Add(1, 2);
                TheoryMembershipSignificanceCopier.ColumnMappings.Add(2, 3);
                TheoryMembershipSignificanceCopier.ColumnMappings.Add(3, 4);
                TheoryMembershipSignificanceCopier.WriteToServer(SourcesNeedingTheoryMembershipSignificances.Select(sntm => new TheoryMembershipSignificanceBinder(TheoryId, sntm.SourceId, null, false)).AsDataReader());
            }
            else
            {
                Trace.WriteLine("No theory members need initialization", "Informational");
            }
            return r.RunId;
        }

        public void StoreAnalysisResults(int TheoryId, int RunId, SourceWithReferences[] AnalysisEntriesToCommit)
        {
            SqlBulkCopy TheoryMembershipCommitter = new SqlBulkCopy(((EntityConnection)Context.Connection).StoreConnection.ConnectionString);
            TheoryMembershipCommitter.DestinationTableName = "TheoryMembership";

            TheoryMembershipCommitter.ColumnMappings.Add(0, 1);
            TheoryMembershipCommitter.ColumnMappings.Add(1, 2);
            TheoryMembershipCommitter.ColumnMappings.Add(2, 3);
            TheoryMembershipCommitter.ColumnMappings.Add(3, 4);
            TheoryMembershipCommitter.ColumnMappings.Add(4, 5);
            TheoryMembershipCommitter.ColumnMappings.Add(5, 6);
            TheoryMembershipCommitter.ColumnMappings.Add(6, 7);
            TheoryMembershipCommitter.ColumnMappings.Add(7, 8);
            TheoryMembershipCommitter.ColumnMappings.Add(8, 9);

            TheoryMembershipCommitter.WriteToServer(AnalysisEntriesToCommit.Select(s => new TheoryMembershipBinder(TheoryId, s.SourceId, RunId, s.Depth, s.ImpactFactor, s.TheoryAttributionRatio, s.ArticleLevelEigenFactor, s.TheoryNamePresent, s.PredictionProbability, s.IsContributingPrediction)).AsDataReader());
        }
    }
}
