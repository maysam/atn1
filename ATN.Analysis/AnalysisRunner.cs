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

        public AnalysisRunner(ATNEntities Entities = null)
        {
            _theories = new Theories(Entities);
            _analysis = new AnalysisInterface(Entities);
        }

        /// <summary>
        /// Run theory analysis and store it in the target database
        /// </summary>
        /// <param name="TheoryId"></param>
        public void AnalyzeTheory(int TheoryId)
        {
            Stopwatch TotalTimer = new Stopwatch();
            Stopwatch Timer = new Stopwatch();
            Timer.Start();
            TotalTimer.Start();

            Trace.WriteLine("Creating theory tree", "Informational");
            Theory TheoryToAnalyze = _theories.GetTheory(TheoryId);
            Dictionary<long, SourceWithReferences> SourceTree = _theories.GetSourceTreeForTheory(TheoryId);
            Trace.WriteLine(string.Format("Theory tree created in {0}", Timer.Elapsed), "Informational");
            Timer.Restart();

            //Create any missing TheoryMembershipSignificance rows

            Trace.WriteLine("Initiating theory analysis", "Informational");
            int RunId = _analysis.InitializeTheoryAnalysis(TheoryId, SourceTree.Values.ToArray());
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

                if (TheoryToAnalyze.TheoryAttributionRatio)
                {
                    //Compute TAR
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

        }
    }
}
