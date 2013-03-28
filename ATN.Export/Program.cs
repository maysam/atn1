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
            Source[] CanonicalSources = t.GetCanonicalSourcesForTheory(7);

            Sources Sources = new Sources();

            //This works by building a graph of all possible nodes in the graph such
            //that ImpactFactor scores can be properly computed. Once all nodes have
            //been added to the graph, all citations that are not in the graph are
            //removed before edge objects are created

            //Build raw list of all possible edges in the graph
            foreach (Source CanonicalSource in CanonicalSources)
            {
                var CitingSources = CanonicalSource.CitingSources;

                AddSourceToGraph(CanonicalSource, CitingSources.Select(src => src.SourceId).ToArray());

                //Write citation nodes/edges
                foreach (Source CitingSource in CitingSources)
                {
                    AddSourceToGraph(CitingSource, CitingSource.CitingSources.Select(src => src.SourceId).ToArray());

                    //Write reference nodes/edges
                    foreach (Source ReferenceSource in CitingSource.References)
                    {
                        AddSourceToGraph(ReferenceSource, ReferenceSource.CitingSources.Select(src => src.SourceId).ToArray());
                    }
                }
            }

            //Prune raw list to remove citations which are not present
            long[] AllKeys = SourceIdCitedBy.Keys.ToArray();
            foreach (long SourceId in AllKeys)
            {
                List<long> CitedBySourceIds = new List<long>(SourceIdCitedBy[SourceId].Count);
                foreach (long CitedBySourceId in SourceIdCitedBy[SourceId])
                {
                    if (SourceIdCitedBy.ContainsKey(CitedBySourceId) && CitedBySourceId != SourceId)
                    {
                        CitedBySourceIds.Add(CitedBySourceId);
                    }
                }
                SourceIdCitedBy[SourceId] = CitedBySourceIds;
            }

            foreach (KeyValuePair<long, List<long>> SourceAndCitations in SourceIdCitedBy)
            {
                foreach (long Citation in SourceAndCitations.Value)
                {
                    GetIndexForSource(SourceAndCitations.Key);
                    GetIndexForSource(Citation);
                    Edges.Add(new SourceEdge(Citation, SourceAndCitations.Key));
                }
            }
            Edges = Edges.OrderBy(e => e.StartSourceId).ThenBy(e => e.EndSourceId).ToList();

            foreach (KeyValuePair<long, long> KV in IndexToSourceId)
            {
                Nodes.Add(new SourceNode(KV.Value, "Title", SourceIdCitedBy[KV.Value].Count));
            }
            FileStream DestinationNetStream = File.Open("Graph.net", FileMode.Create);
            PajekDotNetExporter.Export(Nodes.ToArray(), Edges.ToArray(), DestinationNetStream);

        }
    }
}
