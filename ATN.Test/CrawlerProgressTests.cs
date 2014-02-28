using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ATN.Data;
using ATN.Crawler.WebCrawler;

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
        public void VerifyWokCrawl()
        {
            WOKCrawler wok = new WOKCrawler();

            //ut=A1989CC00400006 or ut=A1989AL89000005
            Assert.AreEqual(wok.GetCitationsBySourceId("A1989AL89000005").Length, 2665); // not 1619 , books are not added
            Assert.AreEqual(wok.GetCitationsBySourceId("A1989CC00400006").Length, 4145); // not 1619 , books are not added
            string testid = "000073360600013";
            string[] citations_testid = wok.GetCitationsBySourceId(testid);
            Assert.AreEqual(citations_testid.Length, 1529); // not 1619 , books are not added
            
            string child1234ID = "A1997WT80400003";
            string[] citations_1234 = wok.GetCitationsBySourceId(child1234ID);
            Assert.AreEqual(citations_1234.Length, 1171); // not 1234 , books are not added
            
            string parentID = "000240045200008";
            string childID = "000272014500002";
            string[] citations = wok.GetCitationsBySourceId(parentID);
            Assert.IsTrue(citations.Contains<string>(childID));
            string problemID = "000253225600012";
            try
            {
                CompleteSource cs = wok.GetSourceById(problemID);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Invalid Source");
            }

            string problemID_2 = "1910882";
            try
            {
                CompleteSource cs_2 = wok.GetSourceById(problemID_2);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, "Invalid Source");
            }

            string[] references = wok.GetReferencesBySourceId(childID);
            Assert.AreEqual(references.Length, 0);
        }


        [TestMethod]
        public void VerifyQueueItemsAreDeleted()
        {
            int CurrentCount = Context.CrawlQueues.Count();

            CrawlQueue cq = CreateCrawlQueue(false);

            Context.CrawlQueues.AddObject(cq);
            Context.SaveChanges();
            CrawlProgress.RemoveInterruptedQueueItems(cq.CrawlId);

            Assert.AreEqual(CurrentCount, Context.CrawlQueues.Count(), "Crawl queue not empty");

        }

        [TestMethod]
        public void VerifyLastReferencedSourceId()
        {
            Crawl c = CreateCrawl(true);
            CrawlProgress.UpdateCrawlerLastEnumeratedSource(c, 1234);
            Assert.AreEqual(c.LastEnumeratedSourceId, CrawlProgress.GetLastSourceIdReferencedInCrawl(c.CrawlId), "Last referenced source id incorrect");

            DeleteCrawl(c.CrawlId);
        }

        [TestMethod]
        public void VerifyExistingCrawls()
        {
            List<Crawl> Crawls = new List<Crawl>(3);

            DateTime Now = DateTime.Now.AddYears(1);

            Crawls.Add(CreateCrawl(Now.AddDays(1), true));
            Crawls.Add(CreateCrawl(Now.AddDays(2), true));
            Crawls.Add(CreateCrawl(Now.AddDays(3), true));

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
            Crawl Crawl = CreateCrawl(true);
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
            Crawl c = CreateCrawl(true);
            Source s = CreateSource(true);
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
