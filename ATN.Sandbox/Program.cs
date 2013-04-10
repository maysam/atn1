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
using ATN.Import;

namespace ATN.Analysis
{
    class Program
    {
        public static void Main(string[] args)
        {
            Theories t = new Theories();
            List<ExtendedSource> AllSourcesForTheory = t.GetAllExtendedSourcesForTheory(2, 0, Int32.MaxValue);

            int CountContrib = AllSourcesForTheory.Count(c => c.Contributing.HasValue && c.Contributing.Value);
            int CountNotContrib = AllSourcesForTheory.Count(c => c.Contributing.HasValue && !c.Contributing.Value);
            int CountNull = AllSourcesForTheory.Count(c => !c.Contributing.HasValue);

            var Things = AllSourcesForTheory.Where(ast => ast.AEF.HasValue && ast.TAR.HasValue);

            int x = 0;

            //FileStream fs = File.Open(@"D:\users\pfaffj\Documents\Visual Studio 2012\Projects\atn\ATN.Sandbox\bin\x64\Debug\TAM - Training Data.csv", FileMode.Open, FileAccess.Read);
            //ImportManualMetaAnalysis Importer = new ImportManualMetaAnalysis();
            //Importer.ImportTheory(2, fs);

            //int TheoryId = 2;
            //AnalysisRunner ar = new AnalysisRunner();
            //CrawlerProgress cp = new CrawlerProgress();
            //Crawl c = cp.GetCrawls().SingleOrDefault(ic => ic.TheoryId == TheoryId);
            //ar.AnalyzeTheory(c, TheoryId);
        }
    }
}