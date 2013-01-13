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
            CrawlerProgress Progress = new CrawlerProgress();
            string[] MasIds = Progress.GetCanonicalIdsForCrawl(CrawlerDataSource.MicrosoftAcademicSearch, 2);
            Sources s = new Sources();
            Source CanonicalSource = s.GetSourceByDataSourceSpecificId(CrawlerDataSource.MicrosoftAcademicSearch, MasIds.First());

            Dictionary<long, SourceNode> Nodes = new Dictionary<long, SourceNode>(CanonicalSource.CitingSources.Count * 50);
            List<SourceEdge> Edges = new List<SourceEdge>(CanonicalSource.CitingSources.Count * 50 * 2);

            //Write canonical node
            Nodes.Add(CanonicalSource.SourceId, new SourceNode(CanonicalSource.SourceId, CanonicalSource.ArticleTitle, CanonicalSource.CitingSources.Count));

            //Write citation nodes
            foreach (var Source in CanonicalSource.CitingSources)
            {
                if (!Nodes.ContainsKey(Source.SourceId))
                {
                    Nodes.Add(Source.SourceId, new SourceNode(Source.SourceId, Source.ArticleTitle, Source.CitingSources.Count));
                }
            }

            //Write reference nodes
            var Citations = CanonicalSource.CitingSources.ToArray();
            for(int i = 0; i < Citations.Length; i++)
            {
                Console.WriteLine("Writing references for citation {0}/{1}", i + 1, Citations.Length);
                var References = Citations[i].References.ToArray();
                for (int j = 0; j < References.Length; j++)
                {
                    if (!Nodes.ContainsKey(References[j].SourceId))
                    {
                        Nodes.Add(References[j].SourceId, new SourceNode(References[j].SourceId, References[j].ArticleTitle, References[j].CitingSources.Count));
                    }
                }
            }

            //Write citation edges
            foreach (var Citation in CanonicalSource.CitingSources)
            {
                if (Citation.SourceId != CanonicalSource.SourceId)
                {
                    Edges.Add(new SourceEdge(Citation.SourceId, CanonicalSource.SourceId));
                }
            }

            //Write reference nodes
            foreach (var Citation in CanonicalSource.CitingSources)
            {
                foreach (var Reference in Citation.References)
                {
                    if (Citation.SourceId != Reference.SourceId)
                    {
                        Edges.Add(new SourceEdge(Citation.SourceId, Reference.SourceId));
                    }
                }
            }

            FileStream DestinationXMLStream = File.Open("Graph.xml", FileMode.Create);
            XGMMLExporter.Export(Nodes.Values.ToArray(), Edges.ToArray(), DestinationXMLStream);
            DestinationXMLStream.Close();

            FileStream DestinationNetStream = File.Open("Graph.net", FileMode.Create);
            PajekDotNetExporter.Export(Nodes.Values.ToArray(), Edges.ToArray(), DestinationNetStream);
            DestinationNetStream.Close();
        }
    }
}
