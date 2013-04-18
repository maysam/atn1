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
            //int TheoryId = 6;

            //AnalysisRunner ar = new AnalysisRunner();
            //CrawlerProgress cp = new CrawlerProgress();
            //Crawl c = cp.GetCrawls().SingleOrDefault(ic => ic.TheoryId == TheoryId);
            //ar.AnalyzeTheory(c, TheoryId);

            //int x = 0;

            //FileStream fs = File.Open(@"D:\users\pfaffj\Documents\Visual Studio 2012\Projects\atn\ATN.Sandbox\bin\x64\Debug\TAM - Training Data.csv", FileMode.Open, FileAccess.Read);
            //ImportManualMetaAnalysis Importer = new ImportManualMetaAnalysis();
            //Importer.ImportTheory(2, fs);

            //int TheoryId = 2;
            //AnalysisRunner ar = new AnalysisRunner();
            //CrawlerProgress cp = new CrawlerProgress();
            //Crawl c = cp.GetCrawls().SingleOrDefault(ic => ic.TheoryId == TheoryId);
            //ar.AnalyzeTheory(c, TheoryId);

            int TheoryId = 2;

            GraphBuilder gb = new GraphBuilder();
            Graph UnprunedGraph = gb.GetGraphForTheory(TheoryId, false, false, false, false, false);
            Graph PrunedGraph = gb.GetGraphForTheory(TheoryId, true, true, true, true, true);

            FileStream UnprunedStream = File.Open(TheoryId.ToString() + "UnprunnedGraph.xml", FileMode.Create);
            XGMMLExporter.Export(UnprunedGraph.Nodes.ToArray(), UnprunedGraph.Edges.ToArray(), UnprunedStream);

            FileStream PrunedStream = File.Open(TheoryId.ToString() + "PrunnedGraph.xml", FileMode.Create);
            XGMMLExporter.Export(PrunedGraph.Nodes.ToArray(), PrunedGraph.Edges.ToArray(), PrunedStream);
        }
    }
}