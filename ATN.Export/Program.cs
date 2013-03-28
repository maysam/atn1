using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATN.Data;
using ATN.Analysis;
using System.Xml;
using System.IO;

namespace ATN.Export
{
    public class Program
    {
        static Dictionary<long, long> SourceIdToIndex = new Dictionary<long, long>();
        static Dictionary<long, long> IndexToSourceId = new Dictionary<long, long>();
        static long CurrentSourceIndex = 0;

        static Dictionary<long, List<long>> SourceIdCitedBy = new Dictionary<long, List<long>>();
        static List<SourceEdge> Edges = new List<SourceEdge>();
        static List<SourceNode> Nodes = new List<SourceNode>();

        static long GetIndexForSource(long SourceId)
        {
            if (!SourceIdToIndex.ContainsKey(SourceId))
            {
                SourceIdToIndex[SourceId] = CurrentSourceIndex;
                IndexToSourceId[CurrentSourceIndex] = SourceId;
                CurrentSourceIndex++;
                return CurrentSourceIndex - 1;
            }
            else
            {
                return SourceIdToIndex[SourceId];
            }
        }

        static void AddSourceToGraph(Source Source, long[] Citations)
        {
            SourceIdCitedBy[Source.SourceId] = Citations.ToList();
        }
        static void Main(string[] args)
        {
            Theories t = new Theories();
            ExportSource[] CanonicalSources = t.GetExportSourcesForTheory(2);

            FileStream DestinationNetStream = File.Open("TAM.csv", FileMode.Create);
            StreamWriter sw = new StreamWriter(DestinationNetStream);
            sw.WriteLine("Source ID,MAS ID,Title,Year,Authors,Journal,Contributing (Yes/No),Is Meta Analysis (Yes/No),Meta Analysis Members (MAS IDs)");
            foreach (ExportSource cs in CanonicalSources)
            {
                sw.Write(cs.SourceId + ",");
                sw.Write(cs.MasID + ",");
                sw.Write(string.Format("\"{0}\"", cs.Title.Replace("\"", "\"\"")) + ",");
                sw.Write(cs.Year + ",");
                if (cs.Authors != null)
                {
                    sw.Write(string.Format("\"{0}\"", cs.Authors.Remove(cs.Authors.LastIndexOf(","), 1).Replace("\"", "\"\"")) + ",");
                }
                else
                {
                    sw.Write("None Available,");
                }
                if (cs.Journal != null)
                {
                    sw.Write(string.Format("\"{0}\"", cs.Journal.Replace("\"", "\"\"")) + ",");
                }
                else
                {
                    sw.Write("None,");
                }
                sw.Write(" ,");
                sw.Write(" , ");
                sw.Write(Environment.NewLine);
            }
            sw.Close();
        }
    }
}
