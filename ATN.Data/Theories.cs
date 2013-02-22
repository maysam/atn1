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
        public Theories()
        {
            
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
            Sources s = new Sources();
            TheoryDefinition[] CanonicalPapers = Context.Theories.Single(t => t.TheoryId == TheoryId).TheoryDefinitions.ToArray();
            List<Source> Sources = new List<Source>(CanonicalPapers.Length);
            foreach (TheoryDefinition t in CanonicalPapers)
            {
                Sources.Add(s.GetSourceByDataSourceSpecificIds((CrawlerDataSource)t.DataSourceId, t.CanonicalIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)));
            }
            return Sources.ToArray();
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
        /// Retrieves the sources which cite the canonical papers for a given theory
        /// </summary>
        /// <param name="TheoryId">The theory which to retrieve first-level sources</param>
        /// <returns>All first level sources for the given theory</returns>
        public Source[] GetFirstLevelSourcesForTheory(int TheoryId)
        {
            Sources s = new Sources();
            TheoryDefinition[] CanonicalPapers = Context.Theories.Single(t => t.TheoryId == TheoryId).TheoryDefinitions.ToArray();
            List<Source> Sources = new List<Source>(CanonicalPapers.Length);
            foreach (TheoryDefinition t in CanonicalPapers)
            {
                Source CanonicalSource = s.GetSourceByDataSourceSpecificIds((CrawlerDataSource)t.DataSourceId, t.CanonicalIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                foreach (Source CitingSource in CanonicalSource.CitingSources)
                {
                    Sources.Add(CitingSource);
                }
            }
            return Sources.ToArray();
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
            }

            ContributionSignificance.RAMarkedContributing = RAMarkedContributing;
            Context.SaveChanges();
        }
    }
}
