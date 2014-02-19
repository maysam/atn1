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
            int TheoryId = 2;

            AnalysisRunner ar = new AnalysisRunner();
            CrawlerProgress cp = new CrawlerProgress();
            Crawl c = cp.GetCrawls().SingleOrDefault(ic => ic.TheoryId == TheoryId);
            ar.AnalyzeTheory(c, TheoryId);

            //if (args.Length == 2)
            //{
                //FileStream fs = File.Open(args[1], FileMode.Open, FileAccess.Read);
                //FileStream fs = File.Open("TAM Worksheet.csv", FileMode.Open, FileAccess.Read);
                //ImportManualMetaAnalysis Importer = new ImportManualMetaAnalysis();
                //Importer.ImportTheory(Int32.Parse(args[0]), fs);
                //Importer.ImportTheory(2, fs);
            //}
            //else
            //{
            //    Console.WriteLine("Usage: ATN.Sandbox.exe TheoryId ImportFile");
            //}
            //int x = 0;

            //int TheoryId = 2;
            //AnalysisRunner ar = new AnalysisRunner();
            //CrawlerProgress cp = new CrawlerProgress();
            //Crawl c = cp.GetCrawls().SingleOrDefault(ic => ic.TheoryId == TheoryId);
            //ar.AnalyzeTheory(c, TheoryId);
            //int x = 0;
            //int TheoryId = 2;

            //GraphBuilder gb = new GraphBuilder();
            //Graph UnprunedGraph = gb.GetGraphForTheory(TheoryId, false, false, false, false, false);
            //Graph PrunedGraph = gb.GetGraphForTheory(TheoryId, false, false, false, true, true);
            //int x = 0;
            //FileStream UnprunedStream = File.Open(TheoryId.ToString() + "UnprunnedGraph.xml", FileMode.Create);
            //XGMMLExporter.Export(UnprunedGraph.Nodes.ToArray(), UnprunedGraph.Edges.ToArray(), UnprunedStream);

            //FileStream PrunedStream = File.Open(TheoryId.ToString() + "PrunnedGraph.xml", FileMode.Create);
            //XGMMLExporter.Export(PrunedGraph.Nodes.ToArray(), PrunedGraph.Edges.ToArray(), PrunedStream);

            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            sw.WriteLine("test!");
            sw.Close();
            long z = ms.Length;
        }
    }
}