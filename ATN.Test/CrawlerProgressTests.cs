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
            CrawlQueue cq = new CrawlQueue();
            cq.CrawlReferenceDirection = (int)CrawlReferenceDirection.Citation;
            cq.CrawlId = 1;
            cq.CrawQueueId = 1;
            cq.DataSourceId = 1;
            cq.DataSourceSpecificId = "13371337";
            cq.ReferencesSourceId = 1;

            ModelContext.CrawlQueues.AddObject(cq);
            ModelContext.SaveChanges();

            CrawlerProgress CrawlProgress = new CrawlerProgress(ModelContext);
            CrawlProgress.RemoveInterruptedQueueItems(cq.CrawlId);

            Assert.AreEqual(0, ModelContext.CrawlQueues.Count(), "Crawl queue not empty");

        }

        [TestMethod]
        public void VerifyLastReferencedSourceId()
        {
            Crawl c = AddCrawl();

            CrawlerProgress CrawlProgress = new CrawlerProgress(ModelContext);
            Assert.AreEqual(c.LastEnumeratedSourceId, CrawlProgress.GetLastSourceIdReferencedInCrawl(c.CrawlId), "Last referenced source id incorrect");
        }

        [TestMethod]
        public void VerifyExistingCrawls()
        {
            List<Crawl> Crawls = new List<Crawl>(3);
            Crawls.Add(AddCrawl(new DateTime(1), 1));
            Crawls.Add(AddCrawl(new DateTime(3), 3));
            Crawls.Add(AddCrawl(new DateTime(2), 2));

            CrawlerProgress CrawlProgress = new CrawlerProgress(ModelContext);
            Crawl[] ExistingCrawls = CrawlProgress.GetExistingCrawls();

            //Tests sorting, existence, and values
            Assert.AreEqual(Crawls[0], ExistingCrawls[2], "Retrieved Crawl not equal to added Crawl");
            Assert.AreEqual(Crawls[1], ExistingCrawls[0], "Retrieved Crawl not equal to added Crawl");
            Assert.AreEqual(Crawls[2], ExistingCrawls[1], "Retrieved Crawl not equal to added Crawl");
        }
    }
}
