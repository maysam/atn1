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
            int TheoryId = 3;

            GraphBuilder gb = new GraphBuilder();
            Graph ExportGraph = gb.GetGraphForTheory(TheoryId, true, true, true, true);

            FileStream fs = File.Open(TheoryId.ToString() + "-Graph.xml", FileMode.Create);
            XGMMLExporter.Export(ExportGraph.Nodes.ToArray(), ExportGraph.Edges.ToArray(), fs);
        }
    }
}