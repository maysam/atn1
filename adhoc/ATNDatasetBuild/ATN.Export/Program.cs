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
            string[] MasIds = Progress.GetCanonicalIdsForCrawl(CrawlerDataSource.MicrosoftAcademicSearch, 1);
            SourceRetrievalService Retrival = new SourceRetrievalService();
            Source CanonicalSource = Retrival.GetSourceByDataSourceSpecificId(CrawlerDataSource.MicrosoftAcademicSearch, MasIds.First());

            //Setup XML Writing structures
            FileStream DestinationStream = File.Open("Graph.xml", FileMode.Create);

            //Setup settings
            XmlWriterSettings WriterSettings = new XmlWriterSettings();
            WriterSettings.CheckCharacters = true;
            WriterSettings.ConformanceLevel = ConformanceLevel.Document;
            WriterSettings.Encoding = Encoding.UTF8;
            WriterSettings.Indent = true;
            WriterSettings.IndentChars = "\t";
            WriterSettings.NamespaceHandling = NamespaceHandling.OmitDuplicates;

            //Instantiate writer
            XmlWriter XGMMLWriter = XmlTextWriter.Create(DestinationStream, WriterSettings);

            //Begin writing graph
            XGMMLWriter.WriteStartDocument();

            //Write root element
            XGMMLWriter.WriteStartElement("graph", "http://www.cs.rpi.edu/XGMML");
                XGMMLWriter.WriteAttributeString("label", "ATN Export Test");
                XGMMLWriter.WriteAttributeString("xmlns", "dc", null, "http://purl.org/dc/elements/1.1/");
                XGMMLWriter.WriteAttributeString("xmlns", "xlink", null, "http://www.w3.org/1999/xlink");
                XGMMLWriter.WriteAttributeString("xmlns", "rdf", null, "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
                XGMMLWriter.WriteAttributeString("xmlns", "cy", null, "http://www.cytoscape.org");

            //Write cannonical node
                    XGMMLWriter.WriteStartElement("node");
                    XGMMLWriter.WriteAttributeString("label", CanonicalSource.ArticleTitle);
                    XGMMLWriter.WriteAttributeString("id", CanonicalSource.SourceId.ToString());
                        XGMMLWriter.WriteStartElement("att");
                        XGMMLWriter.WriteAttributeString("name", "size");
                        XGMMLWriter.WriteAttributeString("type", "integer");
                        XGMMLWriter.WriteAttributeString("value", "64");
                        XGMMLWriter.WriteEndElement();
                    XGMMLWriter.WriteEndElement();

            //Write citation nodes
            foreach (var Source in CanonicalSource.CitingSources)
            {
                XGMMLWriter.WriteStartElement("node");
                XGMMLWriter.WriteAttributeString("label", Source.ArticleTitle);
                XGMMLWriter.WriteAttributeString("id", Source.SourceId.ToString());
                    XGMMLWriter.WriteStartElement("att");
                    XGMMLWriter.WriteAttributeString("name", "size");
                    XGMMLWriter.WriteAttributeString("type", "integer");
                    XGMMLWriter.WriteAttributeString("value", "32");
                    XGMMLWriter.WriteEndElement();
                XGMMLWriter.WriteEndElement();
            }

            //Write reference nodes
            foreach (var Citation in CanonicalSource.CitingSources)
            {
                foreach (var Reference in Citation.References)
                {
                    XGMMLWriter.WriteStartElement("node");
                    XGMMLWriter.WriteAttributeString("label", Reference.ArticleTitle);
                    XGMMLWriter.WriteAttributeString("id", Reference.SourceId.ToString());
                        XGMMLWriter.WriteStartElement("att");
                        XGMMLWriter.WriteAttributeString("name", "size");
                        XGMMLWriter.WriteAttributeString("type", "integer");
                        XGMMLWriter.WriteAttributeString("value", "16");
                        XGMMLWriter.WriteEndElement();
                    XGMMLWriter.WriteEndElement();
                }
            }

            //Write citation edges
            foreach (var Citation in CanonicalSource.CitingSources)
            {
                XGMMLWriter.WriteStartElement("edge");
                    XGMMLWriter.WriteAttributeString("label", Citation.SourceId.ToString() + "-" + CanonicalSource.SourceId.ToString());
                    XGMMLWriter.WriteAttributeString("source", Citation.SourceId.ToString());
                    XGMMLWriter.WriteAttributeString("target", CanonicalSource.SourceId.ToString());
                XGMMLWriter.WriteEndElement();
            }

            //Write reference nodes
            foreach (var Citation in CanonicalSource.CitingSources)
            {
                foreach (var Reference in Citation.References)
                {
                    XGMMLWriter.WriteStartElement("edge");
                        XGMMLWriter.WriteAttributeString("label", Citation.SourceId.ToString() + "-" + Reference.SourceId.ToString());
                        XGMMLWriter.WriteAttributeString("source", Citation.SourceId.ToString());
                        XGMMLWriter.WriteAttributeString("target", Reference.SourceId.ToString());
                    XGMMLWriter.WriteEndElement();
                }
            }

            //Write root element end
            XGMMLWriter.WriteEndElement();
            XGMMLWriter.WriteEndDocument();

            //Cleanup
            XGMMLWriter.Flush();
            XGMMLWriter.Close();
            DestinationStream.Close();
        }
    }
}
