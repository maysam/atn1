using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ATN.Data;

namespace ATN.Test
{
    /// <summary>
    /// Base class for all tests that interact with data
    /// </summary>
    [TestClass]
    public abstract class DataUnitTestBase : DatabaseInterface
    {
        public DataUnitTestBase() : base(new ATNEntities("name=ATNTest"))
        {

        }

        private TestContext testContextInstance;

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

        protected Crawl CreateCrawl(bool Attach, Theory TheoryToUse = null)
        {
            Crawl c = new Crawl();
            c.CrawlIntervalDays = 7;
            c.CrawlState = (int)CrawlerState.CanonicalPaperComplete;
            c.DateCrawled = DateTime.Now;
            c.LastEnumeratedSourceId = 1234;
            if (TheoryToUse == null)
            {
                Theory t = CreateTheory(DateTime.Now, true);
                c.TheoryId = t.TheoryId;
            }
            else
            {
                c.TheoryId = TheoryToUse.TheoryId;
            }

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
            t.LastModifiedDate = DateTime.Now;

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

        protected Crawl CreateCrawl(DateTime DateAdded, bool Attach, Theory TheoryToUse = null)
        {
            Crawl c = new Crawl();
            c.CrawlIntervalDays = 7;
            c.CrawlState = (int)CrawlerState.ScheduledCrawlEnqueueingReferencesComplete;
            c.DateCrawled = DateAdded;
            c.LastEnumeratedSourceId = 1234;

            if (TheoryToUse == null)
            {
                Theory t = CreateTheory(DateTime.Now, true);
                c.TheoryId = t.TheoryId;
            }
            else
            {
                c.TheoryId = TheoryToUse.TheoryId;
            }

            if (Attach)
            {
                Context.Crawls.AddObject(c);
                Context.SaveChanges();
            }

            return c;
        }

        protected Source CreateSource(bool Attach, string Title = "Test Source")
        {

            Source s = new Source();
            s.DataSourceId = 1;
            s.DataSourceSpecificId = Guid.NewGuid().ToString();
            s.ArticleTitle = Title;
            s.SerializedDataSourceResponse = "<Response />";
            s.Year = DateTime.Now.Year;
            if (Attach)
            {
                Context.Sources.AddObject(s);
                Context.SaveChanges();
            }

            return s;
        }

        protected Journal CreateJournal(bool Attach)
        {
            Journal JournalToAdd = new Journal();
            JournalToAdd.JournalName = "Test Journal";
            if (Attach)
            {
                Context.AddToJournals(JournalToAdd);
                Context.SaveChanges();
            }
            return JournalToAdd;
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

        public void CreateTestTheoryNetwork(out Theory Theory, out Source CanonicalSource, out Source FirstLevelSource,
            out Source SecondLevelSource, out Journal SourceJournal, out Author SourceAuthor, string TheoryName = "Test Theory", string SourceTitles = "Test Source")
        {
            //Create canonical source
            CanonicalSource = CreateSource(true, SourceTitles);

            //Create journal and set canonical source journalid
            SourceJournal = CreateJournal(true);
            CanonicalSource.JournalId = SourceJournal.JournalId;

            //Create author, add to canonical source
            SourceAuthor = CreateAuthor(true);
            AuthorsReference SourceAuthorReference = new AuthorsReference();
            SourceAuthorReference.SourceId = CanonicalSource.SourceId;
            SourceAuthorReference.AuthorId = SourceAuthor.AuthorId;
            Context.AuthorsReferences.AddObject(SourceAuthorReference);
            Context.SaveChanges();

            //Create first and second level sources
            FirstLevelSource = CreateSource(true, SourceTitles);
            SecondLevelSource = CreateSource(true, SourceTitles);

            //Set authors and journals            
            FirstLevelSource.JournalId = SourceJournal.JournalId;
            AuthorsReference FirstLevelAuthorReference = new AuthorsReference();
            FirstLevelAuthorReference.SourceId = FirstLevelSource.SourceId;
            FirstLevelAuthorReference.AuthorId = SourceAuthor.AuthorId;
            Context.AuthorsReferences.AddObject(FirstLevelAuthorReference);
            SecondLevelSource.JournalId = SourceJournal.JournalId;
            AuthorsReference SecondLevelAuthorReference = new AuthorsReference();
            SecondLevelAuthorReference.SourceId = SecondLevelSource.SourceId;
            SecondLevelAuthorReference.AuthorId = SourceAuthor.AuthorId;
            Context.AuthorsReferences.AddObject(SecondLevelAuthorReference);
            Context.SaveChanges();

            CanonicalSource.CitingSources.Add(FirstLevelSource);
            FirstLevelSource.References.Add(SecondLevelSource);
            Context.SaveChanges();

            Theories t = new Theories(Context);
            Theory = t.AddTheory(TheoryName, "Test Theory Comment", true, true, true, true, true, new CanonicalDataSource(CrawlerDataSource.MicrosoftAcademicSearch, CanonicalSource.DataSourceSpecificId));
        }

        protected string GetConcatAuthorString(Source Source)
        {
            return string.Join("", Source.AuthorsReferences.Join(Context.Authors, ar => ar.AuthorId, a => a.AuthorId, (ar, a) => a).Select(a => a.LastName + " " + a.FirstName + ", ").ToArray());
        }

        protected void DeleteSource(long SourceId)
        {
            var TheoryMemberships = Context.TheoryMemberships.Where(tm => tm.SourceId == SourceId);
            foreach (var TheoryMembership in TheoryMemberships)
            {
                Context.DeleteObject(TheoryMembership);
            }
            var TheoryMembershipSignificances = Context.TheoryMembershipSignificances.Where(tms => tms.SourceId == SourceId);
            foreach (var TheoryMembershipSignificance in TheoryMembershipSignificances)
            {
                Context.DeleteObject(TheoryMembershipSignificance);
            }
            var AuthorsReferences = Context.AuthorsReferences.Where(ar => ar.SourceId == SourceId);
            foreach (var AuthorReference in AuthorsReferences)
            {
                Context.DeleteObject(AuthorReference);
            }
            var Source = Context.Sources.SingleOrDefault(s => s.SourceId == SourceId);
            foreach (var Citation in Source.CitingSources.ToArray())
            {
                Source.CitingSources.Remove(Citation);
            }
            foreach (var Reference in Source.References.ToArray())
            {
                Source.References.Remove(Reference);
            }
            Context.DeleteObject(Source);
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
            var AuthorsReferences = Context.AuthorsReferences.Where(ar => ar.AuthorId == Author.AuthorId);
            foreach (var AuthorReference in AuthorsReferences)
            {
                Context.DeleteObject(AuthorReference);
            }
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
            var TheoryMemberships = Context.TheoryMemberships.Where(tm => tm.TheoryId == Theory.TheoryId);
            foreach (var TheoryMembership in TheoryMemberships)
            {
                Context.DeleteObject(TheoryMembership);
            }
            var TheoryMembershipSignificances = Context.TheoryMembershipSignificances.Where(tms => tms.TheoryId == Theory.TheoryId);
            foreach (var TheoryMembershipSignificance in TheoryMembershipSignificances)
            {
                Context.DeleteObject(TheoryMembershipSignificance);
            }
            var Runs = Context.Runs.Where(r => r.TheoryId == Theory.TheoryId);
            foreach (var Run in Runs)
            {
                Context.DeleteObject(Run);
            }
            Context.DeleteObject(Theory);
            Context.SaveChanges();
        }
    }
}
