using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATN.Analysis;
using ATN.Data;
using ATN.Export;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ATN.Test
{
    [TestClass]
    public class ExportTests : DataUnitTestBase
    {
        private AnalysisRunner Runner;
        private AnalysisInterface Analysis;
        private Theories Theories;

        public ExportTests()
        {
            Runner = new AnalysisRunner(Context);
            Analysis = new AnalysisInterface(Context);
            Theories = new Theories(Context);
        }

        [TestMethod]
        public void TestGraphExportation()
        {
            Source AddedSource;
            Source FirstLevelSource;
            Source SecondLevelSource;
            Journal SourceJournal;
            Author SourceAuthor;
            Theory Theory;

            Crawl UnusedCrawl = CreateCrawl(true);

            CreateTestTheoryNetwork(out Theory, out AddedSource, out FirstLevelSource, out SecondLevelSource, out SourceJournal, out SourceAuthor);
            
            var Sources = Theories.GetAllSourcesForTheory(Theory.TheoryId);
            Analysis.InitializeTheoryAnalysis(Theory, Sources);
            Runner.AnalyzeTheory(UnusedCrawl, Theory.TheoryId);

            GraphBuilder Builder = new GraphBuilder(Context);
            Graph ExportGraph = Builder.GetGraphForTheory(Theory.TheoryId, false, false, false, false);
            bool VerifiedCanonical = false;
            bool VerifiedFirstLevel = false;
            bool VerifiedSecondLevel = false;
            foreach (SourceNode n in ExportGraph.Nodes)
            {
                if (n.SourceId == AddedSource.SourceId)
                {
                    Assert.AreEqual(0, n.Depth);
                    Assert.AreEqual(1, n.ImpactFactor);
                    Assert.AreEqual(AddedSource.Year, n.Year);
                    Assert.AreEqual(AddedSource.ArticleTitle, n.Title);
                    VerifiedCanonical = true;
                }
                else if (n.SourceId == FirstLevelSource.SourceId)
                {
                    Assert.AreEqual(1, n.Depth);
                    Assert.AreEqual(0, n.ImpactFactor);
                    Assert.AreEqual(FirstLevelSource.Year, n.Year);
                    Assert.AreEqual(FirstLevelSource.ArticleTitle, n.Title);
                    VerifiedFirstLevel = true;
                }
                else if (n.SourceId == SecondLevelSource.SourceId)
                {
                    Assert.AreEqual(2, n.Depth);
                    Assert.AreEqual(1, n.ImpactFactor);
                    Assert.AreEqual(SecondLevelSource.Year, n.Year);
                    Assert.AreEqual(SecondLevelSource.ArticleTitle, n.Title);
                    VerifiedSecondLevel = true;
                }
            }
            Assert.IsTrue(VerifiedCanonical);
            Assert.IsTrue(VerifiedFirstLevel);
            Assert.IsTrue(VerifiedSecondLevel);

            foreach (SourceEdge e in ExportGraph.Edges)
            {
                    Assert.IsTrue(AddedSource.SourceId == e.EndSourceId || SecondLevelSource.SourceId == e.EndSourceId);
            }

            DeleteTheory(Theory);
            DeleteSource(AddedSource.SourceId);
            DeleteSource(FirstLevelSource.SourceId);
            DeleteSource(SecondLevelSource.SourceId);
        }
    }
}
