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
    public abstract class DataUnitTestBase
    {
        private IUnitOfWork _modelContext;
        public DataUnitTestBase()
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
            _modelContext = new InMemoryUnitOfWork();
        }

        protected Crawl AddCrawl(int CrawlId = 1)
        {
            Crawl c = new Crawl();
            c.CrawlId = CrawlId;
            c.CrawlIntervalDays = 7;
            c.CrawlState = (int)CrawlerState.CanonicalPaperComplete;
            c.DateCrawled = DateTime.Now;
            c.LastEnumeratedSourceId = 1234;
            c.TheoryId = 1;

            ModelContext.Crawls.AddObject(c);
            ModelContext.SaveChanges();

            return c;
        }

        protected Theory AddTheory(DateTime DateAdded)
        {
            Theory t = new Theory();
            t.DateAdded = DateAdded;
            t.TheoryId = 1;
            t.TheoryName = "Test Theory";

            ModelContext.Theories.AddObject(t);
            ModelContext.SaveChanges();

            return t;
        }

        protected Crawl AddCrawl(DateTime DateAdded, int CrawlId = 1)
        {
            Crawl c = new Crawl();
            c.CrawlId = CrawlId;
            c.CrawlIntervalDays = 7;
            c.CrawlState = (int)CrawlerState.CanonicalPaperComplete;
            c.DateCrawled = DateAdded;
            c.LastEnumeratedSourceId = 1234;
            c.TheoryId = 1;

            ModelContext.Crawls.AddObject(c);
            ModelContext.SaveChanges();

            return c;
        }

        public IUnitOfWork ModelContext
        {
            get
            {
                return _modelContext;
            }
        }
    }
}
