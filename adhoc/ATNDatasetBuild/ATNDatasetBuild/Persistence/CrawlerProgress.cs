using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler.Persistence
{
    public enum CrawlerState { Started = 0, CanonicalPaperComplete = 1, EnqueueingCitationsComplete = 2, RetrievingCitationsComplete = 3, EnqueueingReferencesComplete = 4, RetrievingReferencesComplete = 5, Complete = 6 };
    public enum CrawlReferenceDirection { None = 0, Citation = 1, Reference = 2 };
    public class CrawlerProgress : DatabaseInterface
    {
        public CrawlerProgress()
        {

        }
        /// <summary>
        /// Creates the neccessary structures to begin enqueueing crawls
        /// </summary>
        /// <param name="DataSource">The data source for which this crawl is being performed on</param>
        /// <param name="CanonicalIds">The data-source specific id or ids for the canonical papers being crawled</param>
        /// <returns></returns>
        public Crawl StartCrawl(CrawlerDataSource DataSource, string[] CanonicalIds)
        {
            string ConcatedCanonicalIds = string.Join(",", CanonicalIds);

            Crawl PotentiallyExistingCrawl = Context.Crawls.Where(c => c.CanonicalIds == ConcatedCanonicalIds).SingleOrDefault();
            if (PotentiallyExistingCrawl == null)
            {
                PotentiallyExistingCrawl = new Crawl();
                PotentiallyExistingCrawl.CanonicalIds = ConcatedCanonicalIds;
                PotentiallyExistingCrawl.DataSourceId = (int)DataSource;
                PotentiallyExistingCrawl.CrawlState = (int)CrawlerState.Started;

                Context.Crawls.AddObject(PotentiallyExistingCrawl);
                Context.SaveChanges();
            }
            return PotentiallyExistingCrawl;
        }

        public void UpdateCrawlerState(Crawl Crawler, CrawlerState State)
        {
            Crawler.CrawlState = (short)State;
            Context.SaveChanges();
        }

        public void StoreCanonicalResult(int CrawlId, string DataSourceSpecificCanonicalId, long SourceId)
        {
            CrawlResult CanonicalCrawl = new CrawlResult();
            CanonicalCrawl.CrawlId = CrawlId;
            CanonicalCrawl.DataSourceSpecificId = DataSourceSpecificCanonicalId;
            CanonicalCrawl.DateRetreieved = DateTime.Now;
            CanonicalCrawl.SourceId = SourceId;

            Context.CrawlResults.AddObject(CanonicalCrawl);
            Context.SaveChanges();
        }

        public void QueueCrawl(int CrawlId, string[] DataSourceSpecificIds, long? ReferencesSourceId, CrawlReferenceDirection Direction)
        {
            foreach (string DataSourceSpecificId in DataSourceSpecificIds)
            {
                CrawlQueue cq = new CrawlQueue();
                cq.CrawlId = CrawlId;
                cq.DataSourceSpecificId = DataSourceSpecificId;
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
            Context.SaveChanges();
        }

        public CrawlQueue[] GetPendingCrawlsForCrawlId(int CrawlId)
        {
            return Context.CrawlQueues.Where(c => c.CrawlId == CrawlId).ToArray();
        }

        public void CompleteQueueItem(CrawlQueue cq, long SourceId, bool ReferencesRetrieved = false)
        {
            Context.CrawlQueues.DeleteObject(cq);
            CrawlResult cr = new CrawlResult();
            cr.CrawlId = cq.CrawlId;
            cr.DataSourceSpecificId = cq.DataSourceSpecificId;
            cr.DateRetreieved = DateTime.Now;
            cr.SourceId = SourceId;
            cr.ReferencesRetrieved = ReferencesRetrieved;
            Context.CrawlResults.AddObject(cr);
            Context.SaveChanges();
        }
    }
}
