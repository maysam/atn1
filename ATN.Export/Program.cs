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
            StreamWriter sw = new StreamWriter(DestinationNetStream, Encoding.Unicode);
            sw.WriteLine("Source ID\tMAS ID\tTitle\tYear\tAuthors\tJournal\tContributing (Yes/No)\tIs Meta Analysis (Yes/No)\tMeta Analysis Members (MAS IDs)");
            foreach (ExportSource cs in CanonicalSources)
            {
                sw.Write(cs.SourceId + "\t");
                sw.Write(cs.MasID + "\t");
                sw.Write(string.Format("\"{0}\"", cs.Title.Replace("\"", "\"\"")) + "\t");
                if (cs.Year != 0)
                {
                    sw.Write(cs.Year + "\t");
                }
                else
                {
                    sw.Write("None Available\t");
                }
                if (cs.Authors != null)
                {
                    sw.Write(string.Format("\"{0}\"", cs.Authors.Remove(cs.Authors.LastIndexOf(","), 1).Replace("\"", "\"\"")) + "\t");
                }
                else
                {
                    sw.Write("None Available\t");
                }
                if (cs.Journal != null)
                {
                    sw.Write(string.Format("\"{0}\"", cs.Journal.Replace("\"", "\"\"")) + "\t");
                }
                else
                {
                    sw.Write("None\t");
                }
                sw.Write(" \t");
                sw.Write(" \t ");
                sw.Write(Environment.NewLine);
            }
            sw.Close();
        }
    }
}
