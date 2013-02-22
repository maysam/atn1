using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ATN.Data;

namespace ATN.Test
{
    /// <summary>
    /// Summary description for CrawlerProgressTests
    /// </summary>
    [TestClass]
    public class CrawlerProgressTests : DataUnitTestBase
    {
        CrawlerProgress CrawlProgress;
        public CrawlerProgressTests()
        {
            CrawlProgress = new CrawlerProgress(Context);
        }

        [TestMethod]
        public void VerifyQueueItemsAreDeleted()
        {
            int CurrentCount = Context.CrawlQueues.Count();

            CrawlQueue cq = new CrawlQueue();
            cq.CrawlReferenceDirection = (int)CrawlReferenceDirection.Citation;
            cq.CrawlId = 2;
            cq.DataSourceId = 1;
            cq.DataSourceSpecificId = "999999999999";
            cq.ReferencesSourceId = 1;

            Context.CrawlQueues.AddObject(cq);
            Context.SaveChanges();
            CrawlProgress.RemoveInterruptedQueueItems(cq.CrawlId);

            Assert.AreEqual(CurrentCount, Context.CrawlQueues.Count(), "Crawl queue not empty");

        }

        [TestMethod]
        public void VerifyLastReferencedSourceId()
        {
            Crawl c = AddCrawl();
            CrawlProgress.UpdateCrawlerLastEnumeratedSource(c, 1234);
            Assert.AreEqual(c.LastEnumeratedSourceId, CrawlProgress.GetLastSourceIdReferencedInCrawl(c.CrawlId), "Last referenced source id incorrect");

            DeleteCrawl(c.CrawlId);
        }

        [TestMethod]
        public void VerifyExistingCrawls()
        {
            List<Crawl> Crawls = new List<Crawl>(3);

            DateTime Now = DateTime.Now.AddYears(1);

            Crawls.Add(AddCrawl(Now.AddDays(1)));
            Crawls.Add(AddCrawl(Now.AddDays(2)));
            Crawls.Add(AddCrawl(Now.AddDays(3)));

            Crawl[] ExistingCrawls = CrawlProgress.GetExistingCrawls();

            //Tests sorting, existence, and values
            Assert.AreEqual(Crawls[2], ExistingCrawls[0], "Retrieved Crawl not equal to added Crawl");
            Assert.AreEqual(Crawls[1], ExistingCrawls[1], "Retrieved Crawl not equal to added Crawl");
            Assert.AreEqual(Crawls[0], ExistingCrawls[2], "Retrieved Crawl not equal to added Crawl");

            //Cleanup
            foreach (Crawl c in Crawls)
            {
                DeleteCrawl(c.CrawlId);
            }
        }

        [TestMethod]
        public void VerifyStatusIsUpdated()
        {
            Crawl Crawl = AddCrawl();
            CrawlProgress.UpdateCrawlerState(Crawl, CrawlerState.ScheduledCrawlRetrievingCitationsComplete);
            Assert.AreEqual(Crawl.CrawlState, (int)CrawlerState.ScheduledCrawlRetrievingCitationsComplete, "Crawl state was not set properly");

            Crawl.LastEnumeratedSourceId = 1;
            Context.SaveChanges();
            Assert.AreEqual(Crawl.CrawlState, (int)CrawlerState.ScheduledCrawlRetrievingCitationsComplete, "Crawl state was not set properly");

            CrawlProgress.UpdateCrawlerState(Crawl, CrawlerState.Complete);
            Assert.AreEqual(Crawl.CrawlState, (int)CrawlerState.Complete, "Crawl state was not set properly");
            Assert.IsNull(Crawl.LastEnumeratedSourceId, "Last enumerated source was not set null");

            DeleteCrawl(Crawl.CrawlId);
        }

        [TestMethod]
        public void VerifyQueueWorkflow()
        {
            Crawl c = AddCrawl();
            Source s = AddSource();
            CrawlProgress.QueueReferenceCrawl(c.CrawlId, new string[] { s.DataSourceSpecificId }, CrawlerDataSource.MicrosoftAcademicSearch, s.SourceId, CrawlReferenceDirection.Reference);
            CrawlProgress.CommitQueue();
            CrawlQueue[] QueueItems = CrawlProgress.GetPendingCrawlsForCrawlId(c.CrawlId);
            foreach (CrawlQueue cq in QueueItems)
            {
                Assert.AreEqual(c.CrawlId, cq.CrawlId, "CrawlId not equal");
                Assert.AreEqual(s.SourceId, cq.ReferencesSourceId, "Reference Source ID not equal");
                Assert.AreEqual(s.DataSourceSpecificId, cq.DataSourceSpecificId, "Data Source Specific ID not equal");
                Assert.AreEqual((int)CrawlReferenceDirection.Reference, cq.CrawlReferenceDirection, "Reference direction not equal");

                CrawlProgress.CompleteQueueItem(cq, s.SourceId, true);
            }

            QueueItems = CrawlProgress.GetPendingCrawlsForCrawlId(c.CrawlId);
            Assert.AreEqual(0, QueueItems.Length, "Queue items not properly completed");
            Assert.AreEqual(1, Context.CrawlResults.Where(cr => cr.CrawlId == c.CrawlId).Count(), "CrawlQueue not properly completed");

            DeleteCrawl(c.CrawlId);
            DeleteSource(s.SourceId);
        }
    }
}
