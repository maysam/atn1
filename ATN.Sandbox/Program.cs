using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using ATN.Analysis;
using ATN.Export;
using ATN.Data;
using System.IO;
using System.Threading;

namespace ATN.Analysis
{
    class Program
    {
        public static void Main(string[] args)
        {
            int TheoryId = 2;
            ATNEntities e = new ATNEntities();
            AnalysisInterface ai = new AnalysisInterface(e);
            Theories t = new Theories(e);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var SourceTree = t.GetSourceTreeForTheory(TheoryId);
            Trace.WriteLine(string.Format("Source tree enumeration: {0}", sw.Elapsed));
            int RunId = ai.InitiateTheoryAnalysis(TheoryId, true, SourceTree.Values.ToArray());
            AEF AEF = new AEF(e);
            AEF.ComputeAndStoreAEF(TheoryId, RunId, SourceTree);
            sw.Stop();
            Trace.WriteLine(string.Format("Total AEF run time: {0}", sw.Elapsed));
            int x = 0;
        }
    }
}