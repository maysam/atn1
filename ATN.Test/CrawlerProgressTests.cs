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
        public CrawlerProgressTests()
        {

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

            CrawlerProgress CrawlProgress = new CrawlerProgress(Context);
            CrawlProgress.RemoveInterruptedQueueItems(cq.CrawlId);

            Assert.AreEqual(CurrentCount, Context.CrawlQueues.Count(), "Crawl queue not empty");

        }

        [TestMethod]
        public void VerifyLastReferencedSourceId()
        {
            Crawl c = AddCrawl();

            CrawlerProgress CrawlProgress = new CrawlerProgress(Context);
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

            CrawlerProgress CrawlProgress = new CrawlerProgress(Context);
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
    }
}
