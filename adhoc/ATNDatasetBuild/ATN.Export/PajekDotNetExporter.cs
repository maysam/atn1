using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ATN.Export
{
    public static class PajekDotNetExporter
    {
        public static void Export(SourceNode[] Nodes, SourceEdge[] Edges, Stream DestinationStream)
        {
            StreamWriter ExportDestination = new System.IO.StreamWriter(DestinationStream, Encoding.ASCII);
            ExportDestination.WriteLine("*Vertices {0}", Nodes.Length);

            Dictionary<long, int> SourceIdToPosition = new Dictionary<long, int>(Nodes.Length);
            for (int i = 0; i < Nodes.Length; i++)
            {
                ExportDestination.WriteLine("{0} \"{1}\"", i + 1, Nodes[i].SourceId.ToString());
                SourceIdToPosition.Add(Nodes[i].SourceId, i + 1);
            }

            ExportDestination.WriteLine("*Arcs {0}", Edges.Length);
            for (int i = 0; i < Edges.Length; i++)
            {
                ExportDestination.WriteLine("{0} {1}", SourceIdToPosition[Edges[i].StartSourceId], SourceIdToPosition[Edges[i].EndSourceId]);
            }
        }
    }
}
