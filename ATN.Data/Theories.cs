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

        public Source GetSource(long SourceId)
        {
            return Context.Sources.Single(s => s.SourceId == SourceId);
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
        /// Retrieves all of the sources for a theory
        /// </summary>
        /// <param name="TheoryId">The id of the theory to retrieve sources for</param>
        /// <returns>List of Extended Sources</returns>
        public List<ExtendedSource> GetAllSourcesForTheory(int TheoryId)
        {
            List<ExtendedSource> sources = new List<ExtendedSource>();

            return sources;
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
