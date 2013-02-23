using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using ATN.Data;

namespace ATN.Test
{
    [TestClass]
    public class TheoriesTests : DataUnitTestBase
    {
        private Theories Theories;
        public TheoriesTests()
        {
            Theories = new Theories(Context);
        }

        [TestMethod]
        public void VerifyAddTheory()
        {
            Theory BoundTheory = Theories.AddTheory("Test Theory", "Test Theory Comment", new CanonicalDataSource(CrawlerDataSource.MicrosoftAcademicSearch, Guid.NewGuid().ToString()));
            Assert.AreNotEqual(0, BoundTheory.TheoryId, "Theory was not added");

            DeleteTheory(BoundTheory);
        }

        [TestMethod]
        public void VerifyGetCanonicalSourcesForTheory()
        {
            Source AddedSource = CreateSource(true);
            Theory BoundTheory = Theories.AddTheory("Test Theory", "Test Theory Comment", new CanonicalDataSource(CrawlerDataSource.MicrosoftAcademicSearch, AddedSource.DataSourceSpecificId));
            Source CanonicalSourceForTheory = Theories.GetCanonicalSourcesForTheory(BoundTheory.TheoryId).SingleOrDefault();
            Assert.AreEqual(AddedSource, CanonicalSourceForTheory, "Source was not associated with Theory");

            DeleteTheory(BoundTheory);
            DeleteSource(CanonicalSourceForTheory.SourceId);
        }

        [TestMethod]
        public void VerifyGetFirstLevelSourcesForTheory()
        {
            Source CanonicalSource = CreateSource(true);
            Source CitingSource = CreateSource(true);

            CanonicalSource.CitingSources.Add(CitingSource);
            Context.SaveChanges();

            Theory BoundTheory = Theories.AddTheory("Test Theory", "Test Theory Comment", new CanonicalDataSource(CrawlerDataSource.MicrosoftAcademicSearch, CanonicalSource.DataSourceSpecificId));
            Source RetrievedFirstLevelSource = Theories.GetFirstLevelSourcesForTheory(BoundTheory.TheoryId).SingleOrDefault();

            Assert.AreEqual(CitingSource, RetrievedFirstLevelSource, "First level source was not added");

            CanonicalSource.CitingSources.Remove(CitingSource);
            Context.SaveChanges();

            DeleteSource(CitingSource.SourceId);
            DeleteSource(CanonicalSource.SourceId);
        }

        [TestMethod]
        public void VerifyMarkSourceTheoryContribution()
        {
            Theory AddedTheory = CreateTheory(DateTime.Now, true);
            Source AddedSource = CreateSource(true);

            Theories.MarkSourceTheoryContribution(AddedTheory.TheoryId, AddedSource.SourceId, true);

            TheoryMembershipSignificance tms = Context.TheoryMembershipSignificances.SingleOrDefault(t => t.TheoryId == AddedTheory.TheoryId && t.SourceId == AddedSource.SourceId);
            Assert.IsNotNull(tms, "Theory membership significance was not added");
            Assert.AreEqual(true, tms.RAMarkedContributing, "Theory membership significance was wrong value");
        }
    }
}
