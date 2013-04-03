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
            AnalysisRunner ar = new AnalysisRunner();
            CrawlerProgress cp = new CrawlerProgress();
            ar.AnalyzeTheory(cp.GetCrawlById(3), 2);
            int x = 0;
        }
    }
}