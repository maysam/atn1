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

        protected Crawl CreateCrawl(bool Attach)
        {
            Crawl c = new Crawl();
            c.CrawlIntervalDays = 7;
            c.CrawlState = (int)CrawlerState.CanonicalPaperComplete;
            c.DateCrawled = DateTime.Now;
            c.LastEnumeratedSourceId = 1234;
            c.TheoryId = 1;

            if (Attach)
            {
                Context.Crawls.AddObject(c);
                Context.SaveChanges();
            }

            return c;
        }

        protected Theory CreateTheory(DateTime DateAdded, bool Attach)
        {
            Theory t = new Theory();
            t.DateAdded = DateAdded;
            t.TheoryName = Guid.NewGuid().ToString();

            if (Attach)
            {
                Context.Theories.AddObject(t);
                Context.SaveChanges();
            }

            return t;
        }

        protected CrawlQueue CreateCrawlQueue(bool Attach)
        {
            CrawlQueue cq = new CrawlQueue();
            cq.CrawlReferenceDirection = (int)CrawlReferenceDirection.Citation;
            cq.CrawlId = 2;
            cq.DataSourceId = 1;
            cq.DataSourceSpecificId = Guid.NewGuid().ToString();
            cq.ReferencesSourceId = 1;

            if (Attach)
            {
                Context.CrawlQueues.AddObject(cq);
                Context.SaveChanges();
            }

            return cq;
        }

        protected Crawl CreateCrawl(DateTime DateAdded, bool Attach)
        {
            Crawl c = new Crawl();
            c.CrawlIntervalDays = 7;
            c.CrawlState = (int)CrawlerState.ScheduledCrawlEnqueueingReferencesComplete;
            c.DateCrawled = DateAdded;
            c.LastEnumeratedSourceId = 1234;
            c.TheoryId = 1;

            if (Attach)
            {
                Context.Crawls.AddObject(c);
                Context.SaveChanges();
            }

            return c;
        }

        protected Source CreateSource(bool Attach)
        {

            Source s = new Source();
            s.DataSourceId = 1;
            s.DataSourceSpecificId = Guid.NewGuid().ToString();
            s.ArticleTitle = "Test Source";
            s.SerializedDataSourceResponse = "<Response />";

            if (Attach)
            {
                Context.Sources.AddObject(s);
                Context.SaveChanges();
            }

            return s;
        }

        protected Author CreateAuthor(bool Attach)
        {
            Author AuthorToAdd = new Author();
            AuthorToAdd.FirstName = "Test";
            AuthorToAdd.LastName = "Author";
            AuthorToAdd.FullName = "Test Author";
            AuthorToAdd.Email = "testauthor@example.com";
            AuthorToAdd.DataSourceSpecificId = Guid.NewGuid().ToString();
            AuthorToAdd.DataSourceId = 1;
            AuthorToAdd.AuthorId = 1;


            if (Attach)
            {
                Context.Authors.AddObject(AuthorToAdd);
                Context.SaveChanges();
            }

            return AuthorToAdd;
        }

        protected Subject CreateSubject(bool Attach)
        {
            Subject SubjectToAdd = new Subject();
            SubjectToAdd.DataSourceId = 1;
            SubjectToAdd.DataSourceSpecificId = Guid.NewGuid().ToString();
            SubjectToAdd.SubjectText = "Test Subject";

            if (Attach)
            {
                Context.Subjects.AddObject(SubjectToAdd);
                Context.SaveChanges();
            }

            return SubjectToAdd;
        }

        protected void DeleteSource(long SourceId)
        {
            Context.DeleteObject(Context.Sources.SingleOrDefault(s => s.SourceId == SourceId));
            Context.SaveChanges();
        }

        protected void DeleteJournal(Journal Journal)
        {
            Context.DeleteObject(Journal);
            Context.SaveChanges();
        }

        protected void DeleteAuthorsReference(AuthorsReference AuthorsReference)
        {
            Context.DeleteObject(AuthorsReference);
            Context.SaveChanges();
        }

        protected void DeleteAuthor(Author Author)
        {
            Context.DeleteObject(Author);
            Context.SaveChanges();
        }

        protected void DeleteSubject(Subject Subject)
        {
            Context.DeleteObject(Subject);
            Context.SaveChanges();
        }

        protected void DeleteTheory(Theory Theory)
        {
            var TheoryDefs = Context.TheoryDefinitions.Where(td => td.TheoryId == Theory.TheoryId);
            foreach (var TheoryDef in TheoryDefs)
            {
                Context.DeleteObject(TheoryDef);
            }
            Context.DeleteObject(Theory);
            Context.SaveChanges();
        }
    }
}
