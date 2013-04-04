using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATN.Data
{
    /// <summary>
    /// A representation of the progress of a given crawl
    /// </summary>
    public enum CrawlerState { Started = 0, CanonicalPaperComplete = 1, EnqueueingCitationsComplete = 2, RetrievingCitationsComplete = 3,
        EnqueueingReferencesComplete = 4, Complete = 5, ScheduledCrawlStarted = 6, ScheduledCrawlEnqueueingCitationsComplete = 7,
        ScheduledCrawlRetrievingCitationsComplete = 8, ScheduledCrawlEnqueueingReferencesComplete = 9};
    
    /// <summary>
    /// A representation of which direction a crawl reference goes; with None meaning the given queue item is not for a reference relationship
    /// </summary>
    public enum CrawlReferenceDirection { None = 0, Citation = 1, Reference = 2 };

    /// <summary>
    /// A service for intiating crawls, queueing and dequeing crawl items, and completing crawls
    /// </summary>
    public class CrawlerProgress : DatabaseInterface
    {
        public CrawlerProgress(ATNEntities Entities = null)
            : base(Entities)
        {

        }

        public Crawl[] GetCrawls()
        {
            return Context.Crawls.ToArray();
        }

        /// <summary>
        /// Sets a Crawl as having been changed during the course of aa crawl run.
        /// This is used in instances where a crawl may be interupted, such that
        /// at the completion of the crawl the processing service knows to kick off
        /// analysis.
        /// </summary>
        /// <param name="Crawl">The Crawl to be set as having changed</param>
        public void SetCrawlerStateChanged(Crawl Crawl)
        {
            Context.Crawls.Single(c => c.CrawlId == Crawl.CrawlId).HasChanged = true;
            Context.SaveChanges();
        }

        /// <summary>
        /// Sets a Crawl as not having been changed since the last analysis run
        /// </summary>
        /// <param name="Crawl">The Crawl to be set as having not changed</param>
        public void SetCrawlerStateUnchanged(Crawl Crawl)
        {
            Context.Crawls.Single(c => c.CrawlId == Crawl.CrawlId).HasChanged = false;
            Context.SaveChanges();
        }


        public Crawl[] GetCrawlsWithoutAnalysisRuns(Crawl[] Except)
        {
            int[] CrawlIdsWithoutAnalysisRuns = Context.ExecuteStoreQuery<int>("SELECT CrawlId FROM Crawl c WHERE c.TheoryId NOT IN (SELECT DISTINCT TheoryId FROM TheoryMembership)").ToArray();
            return CrawlIdsWithoutAnalysisRuns.Join(Context.Crawls, cid => cid, c => c.CrawlId, (cid, c) => c).Except(Except).ToArray();
        }

        /// <summary>
        /// Removes CrawlQueue items for the given CrawlId. Used when a crawl is interrupted to avoid enqueueing duplicate entries.
        /// </summary>
        /// <param name="CrawlId">The CrawlId to remove CrawlQueue items for</param>
        public void RemoveInterruptedQueueItems(int CrawlId)
        {
            Context.CrawlQueues.Where(cq => cq.CrawlId == CrawlId && cq.CrawlReferenceDirection == (int)CrawlReferenceDirection.Citation).ToList().ForEach(delegate(CrawlQueue cq)
            {
                Context.CrawlQueues.DeleteObject(cq);
            });
            Context.SaveChanges();
        }

        /// <summary>
        /// Retrieve the last enumerated SourceId for the given CrawlId. Useful when a crawl is interrupted to avoid
        /// enqueueing duplicate entries, and to start enqueueing where the previous process left off.
        /// </summary>
        /// <param name="CrawlId">CrawlId to retrieve the last enumerated SourceId for</param>
        /// <returns>The most recently enumerated SourceId for a given Crawl</returns>
        public long GetLastSourceIdReferencedInCrawl(int CrawlId)
        {
            return Context.Crawls.Where(c => c.CrawlId == CrawlId && c.LastEnumeratedSourceId.HasValue).Select(c => c.LastEnumeratedSourceId.Value).SingleOrDefault();
        }

        /// <summary>
        /// Retrieves a list of all existing Crawl items
        /// </summary>
        /// <returns>A list of all existing Crawl items</returns>
        public Crawl[] GetExistingCrawls()
        {
            return Context.Crawls.OrderByDescending(c => c.DateCrawled).ToArray();
        }

        /// <summary>
        /// Returns the crawl specifiers for the given CrawlId
        /// </summary>
        /// <param name="CrawlId">The CrawlId to retrieve the canonical ids for</param>
        /// <returns>All crawl specifiers associated with the given CrawlId</returns>
        public ExistingCrawlSpecifier GetExistingCrawlSpecifierForCrawl(int CrawlId)
        {
            Crawl Crawl = Context.Crawls.Single(c => c.CrawlId == CrawlId);
            TheoryDefinition[] Definition = Context.TheoryDefinitions.Where(td => td.TheoryId == Crawl.TheoryId).ToArray();
            return new ExistingCrawlSpecifier(Crawl, Crawl.Theory.TheoryName, Crawl.Theory.TheoryComment, Crawl.TheoryId, Crawl.Theory.ArticleLevelEigenfactor, Crawl.Theory.ImpactFactor, Crawl.Theory.TheoryAttributionRatio, Crawl.Theory.DataMining, Crawl.Theory.Clustering, Definition);
        }

        /// <summary>
        /// Creates the Crawl entry for the given crawl specifier
        /// </summary>
        /// <param name="CrawlSpecifier">The attributes of the crawl being created</param>
        /// <returns>A persistence-model attached copy of the newly-initiated or previously-initiated crawl</returns>
        public Crawl QueueTheoryCrawl(PendingCrawlSpecifier CrawlSpecifier)
        {
            Theory Theory = Context.Theories.SingleOrDefault(t => t.TheoryId == CrawlSpecifier.TheoryId);
            Crawl PotentiallyExistingCrawl = Theory.Crawl.SingleOrDefault();

            if (PotentiallyExistingCrawl == null)
            {

                PotentiallyExistingCrawl = new Crawl();
                PotentiallyExistingCrawl.TheoryId = CrawlSpecifier.TheoryId;
                PotentiallyExistingCrawl.CrawlState = (int)CrawlerState.Started;
                PotentiallyExistingCrawl.DateCrawled = DateTime.Now;
                PotentiallyExistingCrawl.CrawlIntervalDays = CrawlSpecifier.CrawlIntervalDays;
                Context.Crawls.AddObject(PotentiallyExistingCrawl);
                Context.SaveChanges();
            }
            return PotentiallyExistingCrawl;
        }

        /// <summary>
        /// Update the given Crawl object with the provided CrawlerState
        /// </summary>
        /// <param name="Crawler">Crawl object to update</param>
        /// <param name="State">The new state for the given Crawl object</param>
        public void UpdateCrawlerState(Crawl Crawler, CrawlerState State)
        {
            Crawler.DateCrawled = DateTime.Now;
            Crawler.CrawlState = (short)State;
            if (State != CrawlerState.ScheduledCrawlRetrievingCitationsComplete && State != CrawlerState.RetrievingCitationsComplete)
            {
                Crawler.LastEnumeratedSourceId = null;
            }
            Context.SaveChanges();
        }

        /// <summary>
        /// Update the Crawler with the most recently enumerated SourceId during scheduled crawling
        /// </summary>
        /// <param name="Crawler">Crawl object to update</param>
        /// <param name="State">Last enumerated SourceId</param>
        public void UpdateCrawlerLastEnumeratedSource(Crawl Crawler, long LastEnumeratedSourceId)
        {
            Crawler.DateCrawled = DateTime.Now;
            Crawler.LastEnumeratedSourceId = LastEnumeratedSourceId;
            Context.SaveChanges();
        }

        /// <summary>
        /// Stores the translation between a data-source specific identifier, and it's persistence-model SourceId
        /// </summary>
        /// <param name="CrawlId">The Crawl with which this canonical paper is a part of</param>
        /// <param name="DataSourceSpecificCanonicalId">A canonical paper ID for the given Crawl</param>
        /// <param name="SourceId">The persistence-model SourceId of the given canonical paper ID</param>
        public void StoreCanonicalResult(int CrawlId, string DataSourceSpecificCanonicalId, CrawlerDataSource DataSource, long SourceId)
        {
            CrawlResult CanonicalCrawl = new CrawlResult();
            CanonicalCrawl.CrawlId = CrawlId;
            CanonicalCrawl.DataSourceSpecificId = DataSourceSpecificCanonicalId;
            CanonicalCrawl.DataSourceId = (int)DataSource;
            CanonicalCrawl.DateRetreieved = DateTime.Now;
            CanonicalCrawl.SourceId = SourceId;

            Context.CrawlResults.AddObject(CanonicalCrawl);
            Context.SaveChanges();
        }

        /// <summary>
        /// Queue a list of data-source specific IDs for crawling for the given Crawl. After enqueueing all citations or
        /// references for a crawl, calling the CommitQueue() method is required to commit the changes to the database
        /// </summary>
        /// <param name="CrawlId">The Crawl which these data-source specific IDs pertain to</param>
        /// <param name="DataSourceSpecificIds">The data-source specific IDs to Crawl</param>
        /// <param name="DataSource">The data-source with which the data-source specific IDs are from</param>
        /// <param name="ReferencesSourceId">The referenced persistent-model source, if any</param>
        /// <param name="Direction">The direction of the persistence-model reference, if any</param>
        public void QueueReferenceCrawl(int CrawlId, string[] DataSourceSpecificIds, CrawlerDataSource DataSource, long? ReferencesSourceId, CrawlReferenceDirection Direction)
        {
            foreach (string DataSourceSpecificId in DataSourceSpecificIds)
            {
                CrawlQueue cq = new CrawlQueue();
                cq.CrawlId = CrawlId;
                cq.DataSourceSpecificId = DataSourceSpecificId;
                cq.DataSourceId = (int)DataSource;
                cq.CrawlReferenceDirection = (short)Direction;
                if (Direction != CrawlReferenceDirection.None)
                {
                    if (ReferencesSourceId.HasValue)
                    {
                        cq.ReferencesSourceId = ReferencesSourceId;
                    }
                    else
                    {
                        throw new ArgumentException("Cannot specify a reference direction without specifying a referenced source");
                    }
                }
                else if (ReferencesSourceId.HasValue)
                {
                    throw new ArgumentException("Cannot specify a referenced source without a reference direction");
                }
                Context.CrawlQueues.AddObject(cq);
            }
        }

        /// <summary>
        /// Commits the list of pending crawl queue items
        /// </summary>
        public void CommitQueue()
        {
            var Entries = Context.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Added)
                .Union(Context.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Deleted))
                .Union(Context.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Modified));
            foreach (var Entry in Entries)
            {
                Type t = Entry.Entity.GetType();
                if (t != typeof(CrawlQueue) && t != typeof(CrawlResult))
                {
                    throw new Exception("CrawlQueue commit did not succeed. There were pending changes in entites other than the crawl queue.");
                }
            }
            Context.SaveChanges();
        }

        public Crawl GetCrawlById(int CrawlId)
        {
            return Context.Crawls.Single(c => c.CrawlId == CrawlId);
        }

        /// <summary>
        /// Retrieves a list of pending CrawlQueue items for the given CrawlId
        /// </summary>
        /// <param name="CrawlId">The Crawl to retrieve CrawlQueue items for</param>
        /// <returns>The pending CrawlQueue items for the given CrawlId</returns>
        public CrawlQueue[] GetPendingCrawlsForCrawlId(int CrawlId)
        {
            return Context.CrawlQueues.Where(c => c.CrawlId == CrawlId).ToArray();
        }

        /// <summary>
        /// Deletes the given CrawlQueue item from the queue, and stores the translation between its data-source specific identifier and the persistence-model SourceId
        /// </summary>
        /// <param name="CrawlQueueItem">The CrawlQueue item to mark complete</param>
        /// <param name="SourceId">The persistence-model SourceId corresponding the the given CrawlQueue item</param>
        /// <param name="ReferencesRetrieved">Whether the CrawlQueue item should be marked as having its references retrieved</param>
        public void CompleteQueueItem(CrawlQueue CrawlQueueItem, long SourceId, bool ReferencesRetrieved = false)
        {
            Context.CrawlQueues.DeleteObject(CrawlQueueItem);
            CrawlResult cr = new CrawlResult();
            cr.CrawlId = CrawlQueueItem.CrawlId;
            cr.DataSourceSpecificId = CrawlQueueItem.DataSourceSpecificId;
            cr.DataSourceId = CrawlQueueItem.DataSourceId;
            cr.DateRetreieved = DateTime.Now;
            cr.SourceId = SourceId;
            cr.ReferenceRetrieved = ReferencesRetrieved;
            Context.CrawlResults.AddObject(cr);
            Context.SaveChanges();
        }

        public DateTime GetLastCrawlDate(int theoryId)
        {
            try
            {
                return Context.Crawls.Single(t => t.TheoryId == theoryId).DateCrawled;
            }
            catch (Exception e)
            {
                return DateTime.Now;
            }
        }
    }
}
