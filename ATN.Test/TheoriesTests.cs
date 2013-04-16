using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using ATN.Data;
using ATN.Analysis;

namespace ATN.Test
{
    [TestClass]
    public class TheoriesTests : DataUnitTestBase
    {
        private Theories Theories;
        private AnalysisInterface Analysis;
        private AnalysisRunner Runner;
        public TheoriesTests()
        {
            Theories = new Theories(Context);
            Analysis = new AnalysisInterface(Context);
            Runner = new AnalysisRunner(Context);
        }

        [TestMethod]
        public void VerifyAddTheory()
        {
            Theory BoundTheory = Theories.AddTheory("Test Theory", "Test Theory Comment", true, true, true, true, true, new CanonicalDataSource(CrawlerDataSource.MicrosoftAcademicSearch, Guid.NewGuid().ToString()));
            Assert.AreNotEqual(0, BoundTheory.TheoryId, "Theory was not added");

            DeleteTheory(BoundTheory);
        }

        [TestMethod]
        public void VerifyGetTheory()
        {
            Theory t = CreateTheory(DateTime.Now, true);
            Theory AddedTheory = Theories.GetTheory(t.TheoryId);
            Assert.AreEqual(t.TheoryId, AddedTheory.TheoryId);

            DeleteTheory(AddedTheory);
        }

        [TestMethod]
        public void VerifyGetCanonicalSourcesForTheory()
        {
            Source AddedSource = CreateSource(true);
            Theory BoundTheory = Theories.AddTheory("Test Theory", "Test Theory Comment", true, true, true, true, true, new CanonicalDataSource(CrawlerDataSource.MicrosoftAcademicSearch, AddedSource.DataSourceSpecificId));
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

            Theory BoundTheory = Theories.AddTheory("Test Theory", "Test Theory Comment", true, true, true, true, true, new CanonicalDataSource(CrawlerDataSource.MicrosoftAcademicSearch, CanonicalSource.DataSourceSpecificId));
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
            Source CanonicalSourceForTheory;
            Source FirstLevelSource;
            Source SecondLevelSource;
            Journal SourceJournal;
            Author SourceAuthor;
            Theory BoundTheory;
            CreateTestTheoryNetwork(out BoundTheory, out CanonicalSourceForTheory, out FirstLevelSource, out SecondLevelSource, out SourceJournal, out SourceAuthor);

            var SourceTree = Theories.GetAllSourcesForTheory(BoundTheory.TheoryId);
            Analysis.InitializeTheoryAnalysis(BoundTheory, SourceTree);
            Theories.MarkSourceTheoryContribution(BoundTheory.TheoryId, FirstLevelSource.SourceId, true);

            TheoryMembershipSignificance tms = Context.TheoryMembershipSignificances.SingleOrDefault(t => t.TheoryId == BoundTheory.TheoryId && t.SourceId == FirstLevelSource.SourceId);
            Assert.IsNotNull(tms, "Theory membership significance was not added");
            Assert.AreEqual(true, tms.RAMarkedContributing, "Theory membership significance was wrong value");

            DeleteJournal(SourceJournal);
            DeleteAuthor(SourceAuthor);
            DeleteTheory(BoundTheory);
            DeleteSource(CanonicalSourceForTheory.SourceId);
            DeleteSource(FirstLevelSource.SourceId);
            DeleteSource(SecondLevelSource.SourceId);
        }

        [TestMethod]
        public void VerifyGetExportSourcesForTheory()
        {
            Source CanonicalSourceForTheory;
            Source FirstLevelSource;
            Source SecondLevelSource;
            Journal SourceJournal;
            Author SourceAuthor;
            Theory BoundTheory;
            CreateTestTheoryNetwork(out BoundTheory, out CanonicalSourceForTheory, out FirstLevelSource, out SecondLevelSource, out SourceJournal, out SourceAuthor);

            ExportSource[] Sources = Theories.GetExportSourcesForTheory(BoundTheory.TheoryId);

            bool VerifiedCanonical = false;
            bool VerifiedFirstLevel = false;
            bool VerifiedSecondLevel = false;
            foreach (ExportSource s in Sources)
            {
                Source CompareSource;
                if (s.SourceId == CanonicalSourceForTheory.SourceId)
                {
                    CompareSource = CanonicalSourceForTheory;
                    VerifiedCanonical = true;
                }
                else if (s.SourceId == FirstLevelSource.SourceId)
                {
                    CompareSource = FirstLevelSource;
                    VerifiedFirstLevel = true;
                }
                else
                {
                    CompareSource = SecondLevelSource;
                    VerifiedSecondLevel = true;
                }
                Assert.AreEqual(CompareSource.Journal != null ? CompareSource.Journal.JournalName : null, s.Journal);
                Assert.AreEqual(GetConcatAuthorString(CompareSource), s.Authors);
                Assert.AreEqual(CompareSource.DataSourceSpecificId, s.MasID);
                Assert.AreEqual(CompareSource.ArticleTitle, s.Title);
                Assert.AreEqual(CompareSource.Year, s.Year);
            }

            Assert.IsTrue(VerifiedCanonical);
            Assert.IsTrue(VerifiedFirstLevel);
            Assert.IsTrue(VerifiedSecondLevel);

            DeleteJournal(SourceJournal);
            DeleteAuthor(SourceAuthor);
            DeleteTheory(BoundTheory);
            DeleteSource(CanonicalSourceForTheory.SourceId);
            DeleteSource(FirstLevelSource.SourceId);
            DeleteSource(SecondLevelSource.SourceId);
        }

        [TestMethod]
        public void VerifyGetAllExtendedSources()
        {
            Source CanonicalSourceForTheory;
            Source FirstLevelSource;
            Source SecondLevelSource;
            Journal SourceJournal;
            Author SourceAuthor;
            Theory BoundTheory;
            CreateTestTheoryNetwork(out BoundTheory, out CanonicalSourceForTheory, out FirstLevelSource, out SecondLevelSource, out SourceJournal, out SourceAuthor);
            
            Crawl UnusedCrawl = CreateCrawl(true);

            var SourceTree = Theories.GetAllSourcesForTheory(BoundTheory.TheoryId);

            //We have to do this to create all of the correct values
            Analysis.InitializeTheoryAnalysis(BoundTheory, SourceTree);
            Runner.AnalyzeTheory(UnusedCrawl, BoundTheory.TheoryId);
            List<ExtendedSource> ExtendedSources = Theories.GetAllExtendedSourcesForTheory(BoundTheory.TheoryId);

            bool VerifiedCanonical = false;
            bool VerifiedFirstLevel = false;
            bool VerifiedSecondLevel = false;
            foreach (ExtendedSource s in ExtendedSources)
            {
                Source CompareSource;
                int Depth;
                if (s.SourceId == CanonicalSourceForTheory.SourceId)
                {
                    CompareSource = CanonicalSourceForTheory;
                    Depth = 0;
                    VerifiedCanonical = true;
                }
                else if (s.SourceId == FirstLevelSource.SourceId)
                {
                    CompareSource = FirstLevelSource;
                    Depth = 1;
                    VerifiedFirstLevel = true;
                }
                else
                {
                    CompareSource = SecondLevelSource;
                    Depth = 2;
                    VerifiedSecondLevel = true;
                }
                Assert.AreEqual(CompareSource.Journal != null ? CompareSource.Journal.JournalName : null, s.Journal);
                Assert.AreEqual(GetConcatAuthorString(CompareSource), s.Authors);
                Assert.AreEqual(CompareSource.DataSourceSpecificId, s.MasID);
                Assert.AreEqual(CompareSource.ArticleTitle, s.Title);
                Assert.AreEqual(CompareSource.Year, s.Year);
                Assert.AreEqual(Depth, s.Depth);
                Assert.AreEqual(null, s.Contributing);
                Assert.AreEqual(false, s.IsMetaAnalysis);
                Assert.AreEqual(0, s.NumContributing);
            }

            Assert.IsTrue(VerifiedCanonical);
            Assert.IsTrue(VerifiedFirstLevel);
            Assert.IsTrue(VerifiedSecondLevel);

            DeleteJournal(SourceJournal);
            DeleteAuthor(SourceAuthor);
            DeleteTheory(BoundTheory);
            DeleteSource(CanonicalSourceForTheory.SourceId);
            DeleteSource(FirstLevelSource.SourceId);
            DeleteSource(SecondLevelSource.SourceId);
        }

        [TestMethod]
        public void VerifyGetSourceTreeForTheory()
        {
            Source CanonicalSourceForTheory;
            Source FirstLevelSource;
            Source SecondLevelSource;
            Journal SourceJournal;
            Author SourceAuthor;
            Theory BoundTheory;
            CreateTestTheoryNetwork(out BoundTheory, out CanonicalSourceForTheory, out FirstLevelSource, out SecondLevelSource, out SourceJournal, out SourceAuthor);
            
            var SourceTree = Theories.GetSourceTreeForTheory(BoundTheory.TheoryId);

            bool VerifiedCanonical = false;
            bool VerifiedFirstLevel = false;
            bool VerifiedSecondLevel = false;
            foreach (KeyValuePair<long, SourceWithReferences> Source in SourceTree)
            {
                if (Source.Key == CanonicalSourceForTheory.SourceId)
                {
                    Assert.AreEqual(0, Source.Value.Depth);
                    Assert.AreEqual(0, Source.Value.OutFactor);
                    Assert.AreEqual(1, Source.Value.ImpactFactor);
                    Assert.AreEqual(0, Source.Value.References.Count);
                    VerifiedCanonical = true;
                }
                else if (Source.Key == FirstLevelSource.SourceId)
                {
                    Assert.AreEqual(1, Source.Value.Depth);
                    Assert.AreEqual(2, Source.Value.OutFactor);
                    Assert.AreEqual(0, Source.Value.ImpactFactor);
                    Assert.IsTrue(Source.Value.References.Contains(CanonicalSourceForTheory.SourceId));
                    Assert.IsTrue(Source.Value.References.Contains(SecondLevelSource.SourceId));
                    VerifiedFirstLevel = true;
                }
                else if (Source.Key == SecondLevelSource.SourceId)
                {
                    Assert.AreEqual(2, Source.Value.Depth);
                    Assert.AreEqual(0, Source.Value.OutFactor);
                    Assert.AreEqual(1, Source.Value.ImpactFactor);
                    Assert.AreEqual(0, Source.Value.References.Count);
                    VerifiedSecondLevel = true;
                }
            }
            Assert.IsTrue(VerifiedCanonical);
            Assert.IsTrue(VerifiedFirstLevel);
            Assert.IsTrue(VerifiedSecondLevel);

            DeleteTheory(BoundTheory);
            DeleteSource(CanonicalSourceForTheory.SourceId);
            DeleteSource(FirstLevelSource.SourceId);
            DeleteSource(SecondLevelSource.SourceId);
        }

        [TestMethod]
        public void VerifyGetExtendedSourceReferencesForSource()
        {
            Source AddedSource;
            Source FirstLevelSource;
            Source SecondLevelSource;
            Journal SourceJournal;
            Author SourceAuthor;
            Theory Theory;

            Crawl UnusedCrawl = CreateCrawl(true);
            CreateTestTheoryNetwork(out Theory, out AddedSource, out FirstLevelSource, out SecondLevelSource, out SourceJournal, out SourceAuthor);
            Runner.AnalyzeTheory(UnusedCrawl, Theory.TheoryId);
            List<ExtendedSource> ExtendedSources = Theories.GetExtendedSourceReferencesForSource(Theory.TheoryId, FirstLevelSource.SourceId);
            Assert.AreEqual(2, ExtendedSources.Count);

            foreach (ExtendedSource s in ExtendedSources)
            {
                Source CompareSource;
                int Depth;
                if (s.SourceId == AddedSource.SourceId)
                {
                    CompareSource = AddedSource;
                    Depth = 0;
                }
                else
                {
                    CompareSource = SecondLevelSource;
                    Depth = 2;
                }
                Assert.AreEqual(CompareSource.Journal != null ? CompareSource.Journal.JournalName : null, s.Journal);
                Assert.AreEqual(GetConcatAuthorString(CompareSource), s.Authors);
                Assert.AreEqual(CompareSource.DataSourceSpecificId, s.MasID);
                Assert.AreEqual(CompareSource.ArticleTitle, s.Title);
                Assert.AreEqual(CompareSource.Year, s.Year);
                Assert.AreEqual(Depth, s.Depth);
                Assert.AreEqual(null, s.Contributing);
                Assert.AreEqual(false, s.IsMetaAnalysis);
                Assert.AreEqual(0, s.NumContributing);
            }
        }

        [TestMethod]
        public void VerifyTheoryNamePresence()
        {
            Source AddedSource;
            Source FirstLevelSource;
            Source SecondLevelSource;
            Journal SourceJournal;
            Author SourceAuthor;
            Theory Theory;

            Crawl UnusedCrawl = CreateCrawl(true);
            CreateTestTheoryNetwork(out Theory, out AddedSource, out FirstLevelSource, out SecondLevelSource, out SourceJournal, out SourceAuthor, "TEST", "Source TEST title");

            var SourceTree = Theories.GetSourceTreeForTheory(Theory.TheoryId);
            Analysis.InitializeTheoryAnalysis(Theory, SourceTree.Values.ToArray());

            TheoryMembershipSignificance[] Significances = Context.TheoryMembershipSignificances.Where(s => s.SourceId == AddedSource.SourceId || s.SourceId == FirstLevelSource.SourceId || s.SourceId == SecondLevelSource.SourceId).ToArray();
            foreach (TheoryMembershipSignificance tms in Significances)
            {
                Assert.IsTrue(tms.TheoryNamePresent);
            }

            DeleteTheory(Theory);
            DeleteSource(AddedSource.SourceId);
            DeleteSource(FirstLevelSource.SourceId);
            DeleteSource(SecondLevelSource.SourceId);
        }
    }
}
