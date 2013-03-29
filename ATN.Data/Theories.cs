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
        Sources Sources;
        public Theories(ATNEntities Entities = null) : base(Entities)
        {
            Sources = new Sources(Entities);
        }

        /// <summary>
        /// Adds a Theory
        /// </summary>
        /// <param name="TheoryName">Name of the theory being added</param>
        /// <param name="CanonicalSources">The representation of canonical source identifiers for the theory</param>
        /// <param name="TheoryComment">Any comments relating to the theory</param>
        /// <returns>A persistence-model attached representation of the added theory</returns>
        public Theory AddTheory(string TheoryName, string TheoryComment, params CanonicalDataSource[] CanonicalSources)
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
                CanonicalSources.Add(Sources.GetSourceByDataSourceSpecificIds((CrawlerDataSource)t.DataSourceId, t.CanonicalIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)));
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
                Source CanonicalSource = Sources.GetSourceByDataSourceSpecificIds((CrawlerDataSource)t.DataSourceId, t.CanonicalIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
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
            SourceIdWithDepth[] AllSources = GetAllSourcesForTheory(TheoryId).OrderBy(sid => sid.Depth).ToArray();
            StringBuilder QueryBuilder = new StringBuilder();

            //SQL Server does not receive arrays very easily from a client
            //Consequently, this approach creates a temporary table, loads
            //the maximum allowable number of values per insert query (1000)
            //and then joins against it to select out all of the necessary
            //data for exportation
            QueryBuilder.AppendLine("CREATE TABLE #SourceIdTable (SourceId bigint, Depth smallint);");
            for (int i = 0; i < AllSources.Length / 1000 + 1; i++)
            {
                QueryBuilder.AppendLine("INSERT INTO #SourceIdTable VALUES " + String.Join(",", AllSources.Skip(i * 1000).Take(1000).Select(s => "(" + s.SourceId + "," + s.Depth + ")").ToArray()) + ";");
            }
            QueryBuilder.AppendLine("SELECT s.SourceId as SourceId, DataSourceSpecificId as MasId, ArticleTitle as Title, [Year], (SELECT a.FullName + ', ' as 'data()' FROM AuthorsReference ar, Author a WHERE ar.SourceId = s.SourceId AND ar.AuthorId = a.AuthorId FOR xml path('')) as Authors, j.JournalName as Journal FROM Source s JOIN #SourceIdTable st ON st.SourceId = s.SourceId LEFT OUTER JOIN Journal j ON s.JournalId = j.JournalId ORDER BY st.Depth ASC;");
            QueryBuilder.AppendLine("DROP TABLE #SourceIdTable;");
            ExportSource[] ExportSources = Context.ExecuteStoreQuery<ExportSource>(QueryBuilder.ToString()).ToArray();
            return ExportSources;
        }

        /// <summary>
        /// Retrieves all extended sources that are members of a particular theory
        /// </summary>
        /// <param name="TheoryId">The Theory to retrieve extended sources for</param>
        /// <returns>An array of extended sources which are members of the given theory</returns>
        public ExtendedSource[] GetAllExtendedSourcesForTheory(int TheoryId)
        {
            SourceIdWithDepth[] AllSources = GetAllSourcesForTheory(TheoryId);
            ExtendedSource[] AllExtendedSources = new ExtendedSource[AllSources.Length];
            for (int i = 0; i < AllSources.Length; i++)
            {
                AllExtendedSources[i] = Sources.GetExtendedSourceBySourceId(TheoryId, AllSources[i].SourceId);
            }
            return AllExtendedSources;
        }

        /// <summary>
        /// Retreives a list of source ids, along with the sources they cite and the citation depth
        /// </summary>
        /// <param name="TheoryId">The id of the theory to retrieve sources for</param>
        /// <returns>A dictionary of sources, where the key is the cited source and the list of values are all sources that cite the key along with the citation depth</returns>
        public Dictionary<long, List<SourceIdWithDepth>> GetSourceTreeForTheory(int TheoryId)
        {
            Dictionary<long, List<SourceIdWithDepth>> SourceIdCitedBy = new Dictionary<long, List<SourceIdWithDepth>>();

            Theories t = new Theories(Context);
            Sources Sources = new Sources(Context);

            Source[] CanonicalSources = t.GetCanonicalSourcesForTheory(TheoryId);

            //This retrieves a large table worth of sources, citations, and citation depth
            //It is important that the table is sorted by Depth ascending such that any source
            //which is both a first level and second level source is counted as first level
            //It starts by adding canonical sources into a temporary table, then for each
            //canonical source inserting 1st level sources, and for each 1st level source
            //inserting 2nd level sources
            SourceIdCitedByWithDepth[] SourcesCited = Context.ExecuteStoreQuery<SourceIdCitedByWithDepth>(
                @"CREATE TABLE #SourceIdTable (SourceId bigint, CitesSourceId bigint NULL, Depth SMALLINT);
                INSERT INTO #SourceIdTable SELECT s.SourceId as SourceId, NULL, 0 as Depth FROM Source s WHERE SourceId IN (" + String.Join(",", CanonicalSources.Select(s => s.SourceId.ToString()).ToArray()) + @");
                INSERT INTO #SourceIdTable SELECT c.SourceId as SourceId, st.SourceId, 1 as Depth FROM CitationsReference c JOIN #SourceIdTable st ON st.SourceId = c.CitesSourceId WHERE st.Depth = 0;
                INSERT INTO #SourceIdTable SELECT st.SourceId as SourceId, c.CitesSourceId as CitesSourceId, 2 as Depth FROM CitationsReference c JOIN #SourceIdTable st ON st.SourceId = c.SourceId WHERE st.Depth = 1;
                SELECT * FROM #SourceIdTable ORDER BY Depth ASC
                DROP TABLE #SourceIdTable"
            ).ToArray();

            //This transforms the raw list of citations into a rudimentary tree
            const long CanonicalSourceKey = -1;
            foreach (SourceIdCitedByWithDepth sic in SourcesCited)
            {
                if (sic.CitesSourceId.HasValue)
                {
                    if (!SourceIdCitedBy.ContainsKey(sic.CitesSourceId.Value))
                    {
                        SourceIdCitedBy[sic.CitesSourceId.Value] = new List<SourceIdWithDepth>();
                    }
                    if (sic.CitesSourceId.Value != sic.SourceId)
                    {
                        //This is necessary to avoid instances where a source may be considered 1st level to the canonical source
                        //but may also be considered 2nd level to a separate 1st level source. As the passed table is sorted by
                        //depth the first added source is always the correct depth
                        if (!SourceIdCitedBy[sic.CitesSourceId.Value].Contains(new SourceIdWithDepth(sic.SourceId, sic.Depth), new SourceIdWithDepthComparer()))
                        {
                            SourceIdCitedBy[sic.CitesSourceId.Value].Add(new SourceIdWithDepth(sic.SourceId, sic.Depth));
                        }
                    }
                }
                else
                {
                    //The canonical IDs are included such that any recepient of the tree
                    //can be aware of which papers are canonical and which are not
                    if (!SourceIdCitedBy.ContainsKey(CanonicalSourceKey))
                    {
                        SourceIdCitedBy[CanonicalSourceKey] = new List<SourceIdWithDepth>();
                    }
                    SourceIdCitedBy[CanonicalSourceKey].Add(new SourceIdWithDepth(sic.SourceId, sic.Depth));
                }
            }
            return SourceIdCitedBy;
        }
        class SourceIdWithDepthComparer : IEqualityComparer<SourceIdWithDepth>
        {
            // Products are equal if their names and product numbers are equal. 
            public bool Equals(SourceIdWithDepth x, SourceIdWithDepth y)
            {

                //Check whether the compared objects reference the same data. 
                if (Object.ReferenceEquals(x, y)) return true;

                //Check whether any of the compared objects is null. 
                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;

                //Check whether the products' properties are equal. 
                return x.SourceId == y.SourceId;
            }

            // If Equals() returns true for a pair of objects  
            // then GetHashCode() must return the same value for these objects. 

            public int GetHashCode(SourceIdWithDepth Source)
            {
                //Check whether the object is null 
                if (Object.ReferenceEquals(Source, null)) return 0;

                //Calculate the hash code for the product. 
                return Source.SourceId.GetHashCode();
            }

        }
        public SourceIdWithDepth[] GetAllSourcesForTheory(int TheoryId)
        {
            Dictionary<long, List<SourceIdWithDepth>> SourceIdCitedBy = GetSourceTreeForTheory(TheoryId);
            Dictionary<long, SourceIdWithDepth> AllLevelSourceIds = new Dictionary<long, SourceIdWithDepth>();

            //This enumerates through every value and adds it to the list of sources
            foreach (KeyValuePair<long, List<SourceIdWithDepth>> CurrentLevelSources in SourceIdCitedBy)
            {
                foreach (SourceIdWithDepth CurrentSource in CurrentLevelSources.Value)
                {
                    if (!AllLevelSourceIds.ContainsKey(CurrentSource.SourceId))
                    {
                        AllLevelSourceIds.Add(CurrentSource.SourceId, CurrentSource);
                    }
                }
            }

            //There may exist sources which do not receive any citations; by definition
            //these types of sources have to be 1st level; as 0th level always has citations
            //and 2nd level sources will always be cited by something
            foreach (long SourceId in SourceIdCitedBy.Keys)
            {
                //SourceId == -1 are the canonical sources, which would have already
                //been handled in the previous loop
                if (SourceId != -1 && !AllLevelSourceIds.ContainsKey(SourceId))
                {
                    AllLevelSourceIds.Add(SourceId, new SourceIdWithDepth(SourceId, 0));
                }
            }
            Dictionary<long, int> IFDict = SourceIdCitedBy.Where(s => s.Key != -1).Select(kv => new KeyValuePair<long, int>(kv.Key, kv.Value.Count)).ToDictionary(kv => kv.Key, kv => kv.Value);
            return AllLevelSourceIds.Values.Select(v => new SourceIdWithDepth(v.SourceId, v.Depth, IFDict.ContainsKey(v.SourceId) ? IFDict[v.SourceId] : 0)).OrderBy(src => src.Depth).ToArray();
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
        /// <param name="SourceId"></param>
        /// <returns></returns>
        public Source[] GetReferencesForSource(long SourceId)
        {
            return Context.Sources.Single(s => s.SourceId == SourceId).References.ToArray();
        }

        /// <summary>
        /// Gets the references cited in a certain source. This is used to find sources within a meta-analysis.
        /// </summary>
        /// <param name="SourceId">The id of the source to find references for</param>
        /// <returns>A list of extended Sources that cite the given source</returns>
        public List<ExtendedSource> GetReferencesForSourceId(long SourceId)
        {
            List<ExtendedSource> sources = new List<ExtendedSource>();

            return sources;
        }

        /// <summary>
        /// Marks the given source as being a metaAnalysis paper
        /// </summary>
        /// <param name="TheoryId">The theory the source corresponds to</param>
        /// <param name="SourceId">The source having its metaAnalysis status evaluated</param>
        public void MarkSourceMetaAnalysis(int TheoryId, long SourceId)
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

            MetaAnalysisMembership mam = new MetaAnalysisMembership();
            mam.RAMarkedContributing = Contributing;
            mam.SourceId = MemberSourceId;
            mam.TheoryMembershipSignificanceId = ContributionSignificance.TheoryMembershipSignificanceId;

            Context.MetaAnalysisMemberships.AddObject(mam);
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
            if(ContributionSignificance == null)
            {
                ContributionSignificance = new TheoryMembershipSignificance();
                ContributionSignificance.TheoryId = TheoryId;
                ContributionSignificance.SourceId = SourceId;
                Context.TheoryMembershipSignificances.AddObject(ContributionSignificance);
            }

            ContributionSignificance.RAMarkedContributing = RAMarkedContributing;
            Context.SaveChanges();
        }
    }
}
