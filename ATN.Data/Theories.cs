using System;
using System.Collections.Generic;
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
        public ExtendedSource[] GetAllExtendedSourcesForTheory(int TheoryId)
        {
            SourceWithDepth[] AllSources = GetAllSourcesForTheory(TheoryId);
            ExtendedSource[] AllExtendedSources = new ExtendedSource[AllSources.Length];
            for (int i = 0; i < AllSources.Length; i++)
            {
                AllExtendedSources[i] = Sources.GetExtendedSourceBySourceId(TheoryId, AllSources[i].Source.SourceId);
            }
            return AllExtendedSources;
        }

        /// <summary>
        /// Retrieves all of the sources for a theory
        /// </summary>
        /// <param name="TheoryId">The id of the theory to retrieve sources for</param>
        /// <returns>All Sources in the given theory</returns>
        public SourceWithDepth[] GetAllSourcesForTheory(int TheoryId)
        {
            Dictionary<long, SourceWithDepth[]> SourceIdCitedBy = new Dictionary<long, SourceWithDepth[]>();

            Theories t = new Theories(Context);
            Sources Sources = new Sources(Context);
            Source[] CanonicalSources = t.GetCanonicalSourcesForTheory(TheoryId);

            //This works by building a graph of all possible nodes in the graph such
            //that ImpactFactor scores can be properly computed. Once all nodes have
            //been added to the graph, all citations that are not in the graph are
            //removed

            List<SourceWithDepth> AllLevelSources = new List<SourceWithDepth>();

            //Build raw list of all possible nodes in the graph
            foreach (Source CanonicalSource in CanonicalSources)
            {
                AllLevelSources.Add(new SourceWithDepth(CanonicalSource, 0));

                var CitingSources = CanonicalSource.CitingSources;
                SourceIdCitedBy[CanonicalSource.SourceId] = CitingSources.Select(src => new SourceWithDepth(src, 1)).ToArray();

                //Write citation nodes
                foreach (Source CitingSource in CitingSources)
                {
                    SourceIdCitedBy[CitingSource.SourceId] = CitingSource.CitingSources.Select(src => new SourceWithDepth(src, 1)).ToArray();

                    //Write reference nodes
                    foreach (Source ReferenceSource in CitingSource.References)
                    {
                        SourceIdCitedBy[ReferenceSource.SourceId] = ReferenceSource.CitingSources.Select(src => new SourceWithDepth(src, 2)).ToArray();
                    }
                }
            }

            long[] AllKeys = SourceIdCitedBy.Keys.ToArray();
            foreach (long SourceId in AllKeys)
            {
                List<SourceWithDepth> CitedBySourceIds = new List<SourceWithDepth>(SourceIdCitedBy[SourceId].Length);
                foreach (SourceWithDepth CitedBySource in SourceIdCitedBy[SourceId])
                {
                    if (SourceIdCitedBy.ContainsKey(CitedBySource.Source.SourceId) && CitedBySource.Source.SourceId != SourceId)
                    {
                        CitedBySourceIds.Add(CitedBySource);
                    }
                }
                SourceIdCitedBy[SourceId] = CitedBySourceIds.ToArray();
            }
            
            foreach (SourceWithDepth[] CurrentLevelSources in SourceIdCitedBy.Values)
            {
                foreach (SourceWithDepth CurrentSource in CurrentLevelSources)
                {
                    if (SourceIdCitedBy.ContainsKey(CurrentSource.Source.SourceId))
                    {
                        AllLevelSources.Add(CurrentSource);
                    }
                }
            }

            Dictionary<long, int> IFDict = SourceIdCitedBy.Select(kv => new KeyValuePair<long, int>(kv.Key, kv.Value.Length)).ToDictionary(kv => kv.Key, kv => kv.Value);
            for (int i = 0; i < AllLevelSources.Count; i++)
            {
                AllLevelSources[i].ImpactFactor = IFDict[AllLevelSources[i].Source.SourceId];
            }

            return AllLevelSources.ToArray();
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
            mam.TheoryMembershipSignificanceId = ContributionSignificance.TheoryMembershipSignificanceId     
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
