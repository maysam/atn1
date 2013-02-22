using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ATN.Data;

namespace ATN.Test
{
    /// <summary>
    /// Summary description for DataUnitTestBase
    /// </summary>
    [TestClass]
    public abstract class DataUnitTestBase : DatabaseInterface
    {
        public DataUnitTestBase() : base(new ATNEntities("name=ATNTest"))
        {

        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            
        }

        protected void DeleteCrawl(int CrawlId)
        {
            foreach (var QueueItem in Context.CrawlQueues.Where(cq => cq.CrawlId == CrawlId))
            {
                Context.CrawlQueues.DeleteObject(QueueItem);
            }
            foreach (var CrawlResult in Context.CrawlResults.Where(cq => cq.CrawlId == CrawlId))
            {
                Context.CrawlResults.DeleteObject(CrawlResult);
            }
            Context.Crawls.DeleteObject(Context.Crawls.Where(c => c.CrawlId == CrawlId).SingleOrDefault());
            Context.SaveChanges();
        }

        protected Crawl AddCrawl()
        {
            Crawl c = new Crawl();
            c.CrawlIntervalDays = 7;
            c.CrawlState = (int)CrawlerState.CanonicalPaperComplete;
            c.DateCrawled = DateTime.Now;
            c.LastEnumeratedSourceId = 1234;
            c.TheoryId = 1;

            Context.Crawls.AddObject(c);
            Context.SaveChanges();

            return c;
        }

        protected Theory AddTheory(DateTime DateAdded)
        {
            Theory t = new Theory();
            t.DateAdded = DateAdded;
            t.TheoryId = 1;
            t.TheoryName = "Test Theory";

            Context.Theories.AddObject(t);
            Context.SaveChanges();

            return t;
        }

        protected Crawl AddCrawl(DateTime DateAdded)
        {
            Crawl c = new Crawl();
            c.CrawlIntervalDays = 7;
            c.CrawlState = (int)CrawlerState.ScheduledCrawlEnqueueingReferencesComplete;
            c.DateCrawled = DateAdded;
            c.LastEnumeratedSourceId = 1234;
            c.TheoryId = 1;

            Context.Crawls.AddObject(c);
            Context.SaveChanges();

            return c;
        }

        protected Source AddSource()
        {
            Source s = new Source();
            s.DataSourceId = 1;
            s.DataSourceSpecificId = Guid.NewGuid().ToString();
            s.ArticleTitle = "Test Source";
            s.SerializedDataSourceResponse = "<Response />";
            Context.Sources.AddObject(s);
            Context.SaveChanges();

            return s;
        }

        protected void DeleteSource(long SourceId)
        {
            Context.DeleteObject(Context.Sources.SingleOrDefault(s => s.SourceId == SourceId));
            Context.SaveChanges();
        }
    }
}
