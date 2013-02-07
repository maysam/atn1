using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATN.Data;
using System.Xml;
using System.IO;

namespace ATN.Export
{
    class Program
    {
        static void Main(string[] args)
        {
            Theories Theories = new Theories();
            Source[] CanonicalSources = Theories.GetCanonicalSourcesForTheory(1);

            Dictionary<long, SourceNode> Nodes = new Dictionary<long, SourceNode>();
            List<SourceEdge> Edges = new List<SourceEdge>();

            foreach (Source CanonicalSource in CanonicalSources)
            {
                var CitingSources = CanonicalSource.CitingSources.ToArray();

                //Write canonical node
                Nodes.Add(CanonicalSource.SourceId, new SourceNode(CanonicalSource.SourceId, CanonicalSource.ArticleTitle, CanonicalSource.CitingSources.Count));

                //Write citation nodes
                for (int i = 0; i < CitingSources.Length; i++)
                {
                    if (!Nodes.ContainsKey(CitingSources[i].SourceId))
                    {
                        Console.WriteLine("Writing node {0}/{1}", i + 1, CitingSources.Length);
                        Nodes.Add(CitingSources[i].SourceId, new SourceNode(CitingSources[i].SourceId, CitingSources[i].ArticleTitle, CitingSources[i].CitingSources.Count));
                    }
                }

                //Write reference nodes
                /*for (int i = 0; i < CitingSources.Length; i++)
                {
                    Console.WriteLine("Writing references for node {0}/{1}", i + 1, CitingSources.Length);
                    var References = CitingSources[i].References.ToArray();
                    for (int j = 0; j < References.Length; j++)
                    {
                        if (!Nodes.ContainsKey(References[j].SourceId))
                        {
                            Nodes.Add(References[j].SourceId, new SourceNode(References[j].SourceId, References[j].ArticleTitle, References[j].CitingSources.Count));
                        }
                    }
                }*/

                //Write citation edges
                foreach (var Citation in CitingSources)
                {
                    if (Citation.SourceId != CanonicalSource.SourceId)
                    {
                        Edges.Add(new SourceEdge(Citation.SourceId, CanonicalSource.SourceId));
                    }
                }

                //Write reference edges
                /*foreach (var Citation in CitingSources)
                {
                    foreach (var Reference in Citation.References)
                    {
                        if (Citation.SourceId != Reference.SourceId)
                        {
                            Edges.Add(new SourceEdge(Citation.SourceId, Reference.SourceId));
                        }
                    }
                }*/
            }
            FileStream DestinationXMLStream = File.Open("Graph.xml", FileMode.Create);
            XGMMLExporter.Export(Nodes.Values.ToArray(), Edges.ToArray(), DestinationXMLStream);

            FileStream DestinationNetStream = File.Open("Graph.net", FileMode.Create);
            PajekDotNetExporter.Export(Nodes.Values.ToArray(), Edges.ToArray(), DestinationNetStream);
        }
    }
}
