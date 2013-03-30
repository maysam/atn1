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

namespace ATN.Analysis
{
    class Program
    {
        public static void Main(string[] args)
        {
            AnalysisInterface ai = new AnalysisInterface();
            int RunId = ai.InitiateTheoryAnalysis(7, true);
            AEF AEF = new AEF();
            AEF.ComputeAndStoreAEF(7, RunId);
            int x = 0;
        }
    }
}