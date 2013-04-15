using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ATN.Data;
using ATN.Analysis;
using ATN.Export;

namespace MLSandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            int TheoryId = 2;

            AnalysisRunner ar = new AnalysisRunner();
            CrawlerProgress cp = new CrawlerProgress();
            Crawl c = cp.GetCrawls().SingleOrDefault(ic => ic.TheoryId == TheoryId);
            ar.AnalyzeTheory(c, TheoryId);

            Dictionary<long, Prediction> Classifications = MachineLearning.RunML(TheoryId);
        }
    }
}
