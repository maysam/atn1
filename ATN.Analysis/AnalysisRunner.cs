using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ATN.Data;

namespace ATN.Analysis
{
    /// <summary>
    /// Orchestrates the interaction between individual feature calculations
    /// and stores the final analysis results in the database
    /// </summary>
    public class AnalysisRunner
    {
        private Theories _theories;
        private AnalysisInterface _analysis;
        private CrawlerProgress _progress;
        public AnalysisRunner(ATNEntities Entities = null)
        {
            _theories = new Theories(Entities);
            _analysis = new AnalysisInterface(Entities);
            _progress = new CrawlerProgress(Entities);
        }

        /// <summary>
        /// Run theory analysis and store it in the target database
        /// </summary>
        /// <param name="TheoryId"></param>
        public void AnalyzeTheory(Crawl Crawl, int TheoryId)
        {
            Stopwatch TotalTimer = new Stopwatch();
            Stopwatch Timer = new Stopwatch();
            Timer.Start();
            TotalTimer.Start();

            Trace.WriteLine("Creating theory tree", "Informational");
            Theory TheoryToAnalyze = _theories.GetTheory(TheoryId);
            Dictionary<long, SourceWithReferences> SourceTree = _theories.GetSourceTreeForTheory(TheoryId);
            if (SourceTree.Count == 0)
            {
                return;
            }

            Trace.WriteLine(string.Format("Tree for theory {0} created in {1}", TheoryId, Timer.Elapsed), "Informational");
            Timer.Restart();

            //Create any missing TheoryMembershipSignificance rows

            Trace.WriteLine("Initiating theory analysis", "Informational");
            int RunId = _analysis.InitializeTheoryAnalysis(TheoryToAnalyze, SourceTree.Values.ToArray());
            Trace.WriteLine(string.Format("Theory analysis initiation completed in {0}", Timer.Elapsed), "Informational");
            Timer.Restart();

            //Run AEF
            if (TheoryToAnalyze.ArticleLevelEigenfactor)
            {
                Trace.WriteLine("Running AEF", "Informational");
                Dictionary<long, double> AEFScores = AEF.ComputeAEF(SourceTree);
                foreach (KeyValuePair<long, double> AEFScore in AEFScores)
                {
                    SourceTree[AEFScore.Key].ArticleLevelEigenFactor = AEFScore.Value;
                }
                Trace.WriteLine(string.Format("AEF completed in {0}", Timer.Elapsed));
                Timer.Restart();

                //Run TAR
                if (TheoryToAnalyze.TheoryAttributionRatio)
                {
                    Trace.WriteLine("Running TAR", "Informational");
                    Dictionary<long, double?> TARScores = TAR.ComputeTAR(SourceTree);
                    foreach (KeyValuePair<long, double?> TARScore in TARScores)
                    {
                        SourceTree[TARScore.Key].TheoryAttributionRatio = TARScore.Value;
                    }
                    Trace.WriteLine(string.Format("TAR completed in {0}", Timer.Elapsed));
                    Timer.Restart();
                }
            }

            //Run ML

            //Analysis complete

            //Remove ImpactFactor data if it's not an analysis option
            if (!TheoryToAnalyze.ImpactFactor)
            {
                foreach (KeyValuePair<long, SourceWithReferences> Source in SourceTree)
                {
                    SourceTree[Source.Key].ImpactFactor = null;
                }
            }

            //Store analysis data
            Trace.WriteLine("Storing analysis results");
            _analysis.StoreAnalysisResults(TheoryId, RunId, SourceTree.Values.ToArray());
            Trace.WriteLine(string.Format("Analysis storage completed in {0}", Timer.Elapsed));

            TotalTimer.Stop();
            Trace.WriteLine(string.Format("Analysis run completed in {0}", TotalTimer.Elapsed));

            _progress.SetCrawlerStateUnchanged(Crawl);
        }
    }
}
