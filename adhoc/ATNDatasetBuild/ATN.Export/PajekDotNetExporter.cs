using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATN.Export
{
    public static class PajekDotNetExporter
    {
        public static string Export(SourceNode[] Nodes, SourceEdge[] Edges)
        {
            StringBuilder ExportDestination = new StringBuilder();
            
            ExportDestination.AppendLine(string.Format("*Vertices {0}", Nodes.Length));

            Dictionary<long, int> SourceIdToPosition = new Dictionary<long, int>(Nodes.Length);
            for (int i = 0; i < Nodes.Length; i++)
            {
                ExportDestination.AppendLine(string.Format("{0} \"{1}\" 1.0", i + 1, Nodes[i].Title));
                SourceIdToPosition.Add(Nodes[i].SourceId, i + 1);
            }

            ExportDestination.AppendLine(string.Format("*Arcs {0}", Edges.Length));
            for (int i = 0; i < Edges.Length; i++)
            {
                ExportDestination.AppendLine(string.Format("{0} {1} 1.0", SourceIdToPosition[Edges[i].StartSourceId], SourceIdToPosition[Edges[i].EndSourceId]));
            }

            return ExportDestination.ToString();
        }
    }
}
