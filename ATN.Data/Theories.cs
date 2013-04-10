using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;

namespace ATN.Data
{
    /// <summary>
    /// A service for interacting with theory-related tables
    /// </summary>
    public class Theories : DatabaseInterface
    {
        Sources _sources;
        public Theories(ATNEntities Entities = null) : base(Entities)
        {
            _sources = new Sources(Entities);
        }

        /// <summary>
        /// Adds a Theory
        /// </summary>
        /// <param name="TheoryName">Name of the theory being added</param>
        /// <param name="CanonicalSources">The representation of canonical source identifiers for the theory</param>
        /// <param name="TheoryComment">Any comments relating to the theory</param>
        /// <returns>A persistence-model attached representation of the added theory</returns>
        public Theory AddTheory(string TheoryName, string TheoryComment, bool AEF, bool ImpactFactor, bool TAR, bool DataMining, bool Clustering, params CanonicalDataSource[] CanonicalSources)
        {
            Theory TheoryToAdd = new Theory();
            TheoryToAdd.TheoryName = TheoryName;
            TheoryToAdd.DateAdded = DateTime.Now;
            TheoryToAdd.TheoryComment = TheoryComment;
            Context.Theories.AddObject(TheoryToAdd);
            Context.SaveChanges();

            foreach (CanonicalDataSource CanonicalSource in CanonicalSources)
            {
                TheoryDefinition TheoryCanonicalSource = new TheoryDefinition();
                TheoryCanonicalSource.TheoryId = TheoryToAdd.TheoryId;
                TheoryCanonicalSource.DataSourceId = (int)CanonicalSource.DataSource;
                TheoryCanonicalSource.CanonicalIds = String.Join(",", CanonicalSource.CanonicalIds);
                Context.TheoryDefinitions.AddObject(TheoryCanonicalSource);
            }

            TheoryToAdd.ArticleLevelEigenfactor = AEF;
            TheoryToAdd.Clustering = Clustering;
            TheoryToAdd.DataMining = DataMining;
            TheoryToAdd.ImpactFactor = ImpactFactor;
            TheoryToAdd.TheoryAttributionRatio = TAR;
            Context.SaveChanges();
            return TheoryToAdd;
        }

        /// <summary>
        /// Retrieves a list of canonical sources for the given theory
        /// </summary>
        /// <param name="TheoryId"></param>
        /// <returns></returns>
        public Source[] GetCanonicalSourcesForTheory(int TheoryId)
        {
            TheoryDefinition[] CanonicalPapers = Context.Theories.Single(t => t.TheoryId == TheoryId).TheoryDefinitions.ToArray();
            List<Source> CanonicalSources = new List<Source>(CanonicalPapers.Length);
            foreach (TheoryDefinition t in CanonicalPapers)
            {
                CanonicalSources.Add(_sources.GetSourceByDataSourceSpecificIds((CrawlerDataSource)t.DataSourceId, t.CanonicalIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)));
            }
            return CanonicalSources.ToArray();
        }

        /// <summary>
        /// Retrieves all existing theories
        /// </summary>
        /// <returns>All existing theories</returns>
        public Theory[] GetTheories()
        {
            return Context.Theories.ToArray();
        }

        /// <summary>
        /// Retrieves a specific theory
        /// </summary>
        /// <param name="TheoryId">ID of theory to retrieve</param>
        /// <returns>The requested theory</returns>
        public Theory GetTheory(int TheoryId)
        {
            return Context.Theories.Single(t => t.TheoryId == TheoryId);
        }

        /// <summary>
        /// Retrieves the sources which cite the canonical papers for a given theory
        /// </summary>
        /// <param name="TheoryId">The theory which to retrieve first-level sources</param>
        /// <returns>All first level sources for the given theory</returns>
        public Source[] GetFirstLevelSourcesForTheory(int TheoryId)
        {
            TheoryDefinition[] CanonicalPapers = Context.Theories.Single(t => t.TheoryId == TheoryId).TheoryDefinitions.ToArray();
            List<Source> FirstLevelSources = new List<Source>(CanonicalPapers.Length);
            foreach (TheoryDefinition t in CanonicalPapers)
            {
                Source CanonicalSource = _sources.GetSourceByDataSourceSpecificIds((CrawlerDataSource)t.DataSourceId, t.CanonicalIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                foreach (Source CitingSource in CanonicalSource.CitingSources)
                {
                    FirstLevelSources.Add(CitingSource);
                }
            }
            return FirstLevelSources.ToArray();
        }

        /// <summary>
        /// Retrieves all extended sources that are members of a particular theory
        /// </summary>
        /// <param name="TheoryId">The Theory to retrieve extended sources for</param>
        /// <returns>An array of extended sources which are members of the given theory</returns>
        public ExportSource[] GetExportSourcesForTheory(int TheoryId)
        {
            SourceWithReferences[] AllSources = GetAllSourcesForTheory(TheoryId);
            StringBuilder QueryBuilder = new StringBuilder();

            //SQL Server does not receive arrays very easily from a client
            //Consequently, this approach creates a temporary table, loads
            //the maximum allowable number of values per insert query (1000)
            //and then joins against it to select out all of the necessary
            //data for exportation
            QueryBuilder.AppendLine("CREATE TABLE #SourceIdTable (SourceId bigint, Depth smallint);");
            QueryBuilder.AppendLine("CREATE CLUSTERED INDEX Index_SourceIdTable_SourceId ON #SourceIdTable(SourceId)");
            for (int i = 0; i < AllSources.Length / 1000 + 1; i++)
            {
                if (AllSources.Length - 1000 * i > 0)
                {
                    QueryBuilder.AppendLine("INSERT INTO #SourceIdTable VALUES " + String.Join(",", AllSources.Skip(i * 1000).Take(1000).Select(s => "(" + s.SourceId + "," + s.Depth + ")").ToArray()) + ";");
                }
            }
            QueryBuilder.AppendLine("SELECT s.SourceId as SourceId, DataSourceSpecificId as MasID, ArticleTitle as Title, [Year], (SELECT a.LastName + ' ' + a.FirstName + ', ' as 'data()' FROM AuthorsReference ar, Author a WHERE ar.SourceId = s.SourceId AND ar.AuthorId = a.AuthorId FOR xml path('')) as Authors, j.JournalName as Journal FROM Source s JOIN #SourceIdTable st ON st.SourceId = s.SourceId LEFT OUTER JOIN Journal j ON s.JournalId = j.JournalId ORDER BY st.Depth ASC;");
            QueryBuilder.AppendLine("DROP TABLE #SourceIdTable;");
            ExportSource[] ExportSources = Context.ExecuteStoreQuery<ExportSource>(QueryBuilder.ToString()).ToArray();
            return ExportSources;
        }

        /// <summary>
        /// Retrieves all extended sources that are members of a particular theory
        /// </summary>
        /// <param name="TheoryId">The Theory to retrieve extended sources for</param>
        /// <returns>An array of extended sources which are members of the given theory</returns>
        public List<ExtendedSource> GetAllExtendedSourcesForTheory(int TheoryId, int PageIndex = 0, int PageSize = 200)
        {
            return Context.ExecuteStoreQuery<ExtendedSource>(
                @"WITH TestTable as (
	                SELECT s.SourceId,
                    s.DataSourceSpecificId as MasID,
	                s.ArticleTitle as Title,
	                [Year],
	                (SELECT a.LastName + ' ' + a.FirstName + ', ' as 'data()' FROM AuthorsReference ar, Author a WHERE ar.SourceId = s.SourceId AND ar.AuthorId = a.AuthorId FOR xml path('')) as Authors,
	                j.JournalName as Journal,
	                (SELECT CASE WHEN tms.RAMarkedContributing IS NOT NULL THEN tms.RAMarkedContributing ELSE (
						SELECT CASE WHEN (SELECT COUNT(DISTINCT mm.SourceId) FROM MetaAnalysisMembership mm JOIN TheoryMembershipSignificance tms2 ON mm.TheoryMembershipSignificanceId = tms2.TheoryMembershipSignificanceId AND tms2.IsMetaAnalysis = 1 AND tms2.TheoryId = tms.TheoryId WHERE mm.SourceId = s.SourceId) >= 1 THEN CAST(1 as bit) ELSE (
							SELECT CASE WHEN COUNT(*) >= 1 THEN CAST(0 as bit) ELSE NULL END FROM TheoryMembershipSignificance tms2 JOIN CitationsReference cr ON cr.SourceId = tms2.SourceId WHERE tms2.IsMetaAnalysis = 1 AND tms2.TheoryId = tms.TheoryId  AND cr.CitesSourceId = s.SourceId
						) END
	                ) END) as Contributing,
	                tms.IsMetaAnalysis as IsMetaAnalysis,
	                (SELECT COUNT(mam.MetaAnalysisMembershipId) FROM MetaAnalysisMembership mam WHERE mam.TheoryMembershipSignificanceId = tms.TheoryMembershipSignificanceId) as NumContributing,
	                tm.ArticleLevelEigenfactor AS AEF,
                    tm.TheoryAttributionRatio As TAR,
                    tm.ImpactFactor as ImpactFactor,
                    tm.PredictionProbability as PredictionProbability,
                    tm.isContributingPrediction as IsContributingPrediction,
	                (CASE WHEN tm.Depth IS NULL THEN CAST(3 as smallint) ELSE tm.Depth END) as Depth,
	                ROW_NUMBER() OVER(ORDER BY tm.Depth ASC) As RowNumber FROM Source s LEFT OUTER JOIN TheoryMembershipSignificance tms ON tms.SourceId = s.SourceId LEFT OUTER JOIN Journal j ON s.JournalId = j.JournalId LEFT OUTER JOIN TheoryMembership tm ON tm.TheoryMembershipId = (SELECT TOP 1 TheoryMembershipId FROM TheoryMembership tm WHERE tm.SourceId = tms.SourceId AND tm.TheoryId = tms.TheoryId ORDER BY RunID DESC) WHERE tms.TheoryId = {0}
                )
                SELECT SourceId, MasID, Title, [Year], Authors,Journal, Contributing, IsMetaAnalysis, NumContributing, AEF, TAR, ImpactFactor, Depth FROM TestTable WHERE RowNumber BETWEEN {1} AND {2}",
            TheoryId, PageIndex * PageSize, (PageIndex + 1) * PageSize).ToList();
        }

        /// <summary>
        /// Retreives a list of source ids, along with the sources they cite and the citation depth
        /// </summary>
        /// <param name="TheoryId">The id of the theory to retrieve sources for</param>
        /// <returns>A dictionary of sources, where the key is the cited source and the list of values are all sources that cite the key along with the citation depth</returns>
        public Dictionary<long, SourceWithReferences> GetSourceTreeForTheory(int TheoryId)
        {
            Source[] CanonicalSources = GetCanonicalSourcesForTheory(TheoryId);

            if (CanonicalSources.Length == 0)
            {
                return new Dictionary<long, SourceWithReferences>();
            }

            //This retrieves a large table worth of sources, citations, and citation depth
            //It is important that the table is sorted by Depth ascending such that any source
            //which is both a first level and second level source is counted as first level
            //It starts by adding canonical sources into a temporary table, then for each
            //canonical source inserting 1st level sources, and for each 1st level source
            //inserting 2nd level sources
            //Finally, the select concludes by retrieving all sources and depths; and proceed
            //to select all of the papers which are cited by another source but do not cite
            //anything themselves
            SourceIdCitedByWithDepth[] SourcesCited = Context.ExecuteStoreQuery<SourceIdCitedByWithDepth>(
                string.Format(@"CREATE TABLE #SourceIdTable (SourceId bigint, CitesSourceId bigint NULL, Depth SMALLINT);
                CREATE CLUSTERED INDEX Index_SourceIdTable_SourceId ON #SourceIdTable(SourceId, CitesSourceId)
                INSERT INTO #SourceIdTable SELECT s.SourceId as SourceId, NULL, 0 as Depth FROM Source s WHERE SourceId IN ({0});
                INSERT INTO #SourceIdTable SELECT c.SourceId as SourceId, st.SourceId, 1 as Depth FROM CitationsReference c JOIN #SourceIdTable st ON st.SourceId = c.CitesSourceId WHERE st.Depth = 0;
                INSERT INTO #SourceIdTable SELECT st.SourceId as SourceId, c.CitesSourceId as CitesSourceId, 2 as Depth FROM CitationsReference c JOIN #SourceIdTable st ON st.SourceId = c.SourceId WHERE st.Depth = 1 AND c.CitesSourceId NOT IN (SELECT SourceID FROM #SourceIdTable WHERE Depth = 0)
                SELECT st1.SourceId, st1.CitesSourceId, CAST(st1.Depth as smallint) as Depth, (SELECT COUNT(st2.SourceId) FROM #SourceIdTable st2 WHERE st2.CitesSourceId = st1.SourceId) as ImpactFactor FROM #SourceIdTable st1 UNION
                SELECT st3.CitesSourceId as SourceId, NULL as CitesSourceId, CAST(st3.Depth as smallint) as Depth, (SELECT COUNT(st4.SourceId) FROM #SourceIdTable st4 WHERE st4.CitesSourceId = st3.CitesSourceId) as ImpactFactor FROM #SourceIdTable st3 WHERE st3.CitesSourceId IS NOT NULL AND st3.CitesSourceId NOT IN(SELECT DISTINCT SourceId FROM #SourceIdTable) UNION
                SELECT tms.SourceId as SourceId, NULL as CitesSourceId, CAST(3 as smallint) as Depth, 0 as ImpactFactor FROM TheoryMembershipSignificance tms WHERE tms.TheoryId = {1} AND tms.SourceId NOT IN (SELECT SourceId FROM #SourceIdTable UNION SELECT CitesSourceId FROM #SourceIdTable st WHERE st.CitesSourceId IS NOT NULL) ORDER BY Depth ASC
                DROP TABLE #SourceIdTable", String.Join(",", CanonicalSources.Select(s => s.SourceId.ToString()).ToArray()), TheoryId)
            ).ToArray();

            //Could optimize this by setting the initial count to the distinct count of sources returned from the store query
            Dictionary<long, SourceWithReferences> SourceIdCitedBy = new Dictionary<long, SourceWithReferences>(SourcesCited.Select(s => s.SourceId).Distinct().Count());

            //This transforms the raw list of citations into a rudimentary tree
            foreach (SourceIdCitedByWithDepth sic in SourcesCited)
            {
                if (!SourceIdCitedBy.ContainsKey(sic.SourceId))
                {
                    SourceIdCitedBy[sic.SourceId] = new SourceWithReferences(sic.SourceId, sic.ImpactFactor, sic.Depth);
                }
                if (sic.CitesSourceId.HasValue)
                {
                    if (sic.SourceId != sic.CitesSourceId.Value)
                    {
                        //This is necessary to avoid instances where a source may be considered 1st level to the canonical source
                        //but may also be considered 2nd level to a separate 1st level source. As the passed table is sorted by
                        //depth the first added source is always the correct depth
                        if (!SourceIdCitedBy[sic.SourceId].References.Contains(sic.CitesSourceId.Value))
                        {
                            SourceIdCitedBy[sic.SourceId].AddReference(sic.CitesSourceId.Value);
                        }
                    }
                }
            }
            return SourceIdCitedBy;
        }
        
        /// <summary>
        /// Retrieve all sources for a particular theory
        /// </summary>
        /// <param name="TheoryId">The theory from which to retrieve sources</param>
        /// <returns>All sources for the given theory</returns>
        public SourceWithReferences[] GetAllSourcesForTheory(int TheoryId)
        {
            return GetSourceTreeForTheory(TheoryId).Values.ToArray();
        }

        /// <summary>
        /// Retrieves an array of Sources that reference the specified Source
        /// </summary>
        /// <param name="SourceId"></param>
        /// <returns></returns>
        public Source[] GetCitingSourcesForSource(long SourceId)
        {
            return Context.Sources.Single(s => s.SourceId == SourceId).CitingSources.ToArray();
        }
        /// <summary>
        /// Retrieves an array of Sources that the specified Source references.
        /// </summary>
        /// <param name="SourceId">SourceId to retrieve references for</param>
        /// <returns>A collection of sources which the given source cites</returns>
        public Source[] GetReferencesForSource(long SourceId)
        {
            return Context.Sources.Single(s => s.SourceId == SourceId).References.ToArray();
        }

        /// <summary>
        /// Gets the references for a particular source in a given theory. This is used to find sources within a meta-analysis.
        /// </summary>
        /// <param name="TheoryId">The theory to retrieve extended source references for</param>
        /// <param name="SourceId">The id of the source to find extended source references for</param>
        /// <returns>A list of extended Sources that the given source references</returns>
        public List<ExtendedSource> GetExtendedSourceReferencesForSource(int TheoryId, long SourceId)
        {
            return Context.ExecuteStoreQuery<ExtendedSource>(
                @"SELECT s.SourceId as SourceId,
                s.DataSourceSpecificId as MasID,
                ArticleTitle as Title, [Year],
                (SELECT a.LastName + ' ' + a.FirstName + ', ' as 'data()' FROM AuthorsReference ar, Author a WHERE ar.SourceId = s.SourceId AND ar.AuthorId = a.AuthorId FOR xml path('')) as Authors,
                j.JournalName as Journal,
                tms.RAMarkedContributing as Contributing,
                tms.IsMetaAnalysis as IsMetaAnalysis,
                (SELECT COUNT(MetaAnalysisMembershipId) FROM MetaAnalysisMembership mam WHERE mam.TheoryMembershipSignificanceId = tms.TheoryMembershipSignificanceId) as NumContributing,
                tm.ArticleLevelEigenfactor as AEF,
                tm.TheoryAttributionRatio As TAR,
                tm.PredictionProbability as PredictionProbability,
                tm.isContributingPrediction as IsContributingPrediction,
                tm.Depth as Depth FROM CitationsReference c
                JOIN Source s ON c.CitesSourceId = s.SourceId
                LEFT OUTER JOIN TheoryMembershipSignificance tms ON tms.SourceId = s.SourceId AND tms.TheoryId = {0}
                LEFT OUTER JOIN Journal j ON s.JournalId = j.JournalId
                JOIN TheoryMembership tm ON tm.SourceId = c.CitesSourceId AND
                tm.RunId = (SELECT TOP 1 r.RunId FROM Run r WHERE r.TheoryId = tms.TheoryId ORDER BY r.RunId DESC)
                WHERE c.SourceId = {1}
                ORDER BY SourceId ASC",
                TheoryId, SourceId
            ).ToList();
        }

        /// <summary>
        /// Marks the given source as being a metaAnalysis paper
        /// </summary>
        /// <param name="TheoryId">The theory the source corresponds to</param>
        /// <param name="SourceId">The source having its metaAnalysis status evaluated</param>
        public long MarkSourceMetaAnalysis(int TheoryId, long SourceId)
        {
            TheoryMembershipSignificance ContributionSignificance = Context.TheoryMembershipSignificances.SingleOrDefault(tms => tms.TheoryId == TheoryId && tms.SourceId == SourceId);

            if (ContributionSignificance == null)
            {
                ContributionSignificance = new TheoryMembershipSignificance();
                ContributionSignificance.TheoryId = TheoryId;
                ContributionSignificance.SourceId = SourceId;
                Context.TheoryMembershipSignificances.AddObject(ContributionSignificance);
            }
            ContributionSignificance.IsMetaAnalysis = true;
            Context.SaveChanges();
            return ContributionSignificance.TheoryMembershipSignificanceId;
        }

        public void AddManualTheoryMember(int TheoryId, long SourceId)
        {
            TheoryMembershipSignificance ContributionSignificance = Context.TheoryMembershipSignificances.SingleOrDefault(tms => tms.TheoryId == TheoryId && tms.SourceId == SourceId);

            if (ContributionSignificance == null)
            {
                ContributionSignificance = new TheoryMembershipSignificance();
                ContributionSignificance.TheoryId = TheoryId;
                ContributionSignificance.SourceId = SourceId;
                Context.TheoryMembershipSignificances.AddObject(ContributionSignificance);
            }
            ContributionSignificance.IsMetaAnalysis = false;
            Context.SaveChanges();
        }
        /// <summary>
        /// Marks a given Source as contributing to a meta analysis indicated by the provided TheoryId and MetaAnalysisSourceId
        /// </summary>
        /// <param name="TheoryId">The Theory to mark contribution for</param>
        /// <param name="MetaAnalysisSourceId">The SourceId of the meta analysis</param>
        /// <param name="MemberSourceId">The SourceId of the member of the meta analysis</param>
        /// <param name="Contributing">Whether the provided member Source contributes to the provided meta analysis</param>
        public void MarkMetaAnalysisMember(int TheoryId, long MetaAnalysisSourceId, long MemberSourceId, bool? Contributing)
        {
            TheoryMembershipSignificance ContributionSignificance = Context.TheoryMembershipSignificances.SingleOrDefault(tms => tms.TheoryId == TheoryId && tms.SourceId == MetaAnalysisSourceId && tms.IsMetaAnalysis);

            MetaAnalysisMembership mam = Context.MetaAnalysisMemberships.SingleOrDefault(m => m.TheoryMembershipSignificanceId == ContributionSignificance.TheoryMembershipSignificanceId && m.SourceId == MemberSourceId);
            if (mam == null)
            {
                mam = new MetaAnalysisMembership();
                Context.MetaAnalysisMemberships.AddObject(mam);
            }
            mam.RAMarkedContributing = Contributing;
            mam.SourceId = MemberSourceId;
            mam.TheoryMembershipSignificanceId = ContributionSignificance.TheoryMembershipSignificanceId;

            Context.SaveChanges();
        }

        /// <summary>
        /// Marks the contribution of a theory as determined manually
        /// </summary>
        /// <param name="TheoryId">The theory the source corresponds to</param>
        /// <param name="SourceId">The source having its contribution evaluated</param>
        /// <param name="RAMarkedContributing">Whether or not the source has been marked as contributing to the provided theory</param>
        public void MarkSourceTheoryContribution(int TheoryId, long SourceId, bool? RAMarkedContributing)
        {
            TheoryMembershipSignificance ContributionSignificance = Context.TheoryMembershipSignificances.SingleOrDefault(tms => tms.TheoryId == TheoryId && tms.SourceId == SourceId);
            ContributionSignificance.RAMarkedContributing = RAMarkedContributing;
            Context.SaveChanges();
        }
    }
}
