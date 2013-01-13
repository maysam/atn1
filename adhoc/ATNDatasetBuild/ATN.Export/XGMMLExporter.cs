using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace ATN.Export
{
    public static class XGMMLExporter
    {
        public static void Export(SourceNode[] Nodes, SourceEdge[] Edges, Stream DestinationStream)
        {
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

            //Write nodes
            foreach (var Node in Nodes)
            {
                XGMMLWriter.WriteStartElement("node");
                XGMMLWriter.WriteAttributeString("label", Node.Title);
                XGMMLWriter.WriteAttributeString("id", Node.SourceId.ToString());
                XGMMLWriter.WriteStartElement("att");
                XGMMLWriter.WriteAttributeString("name", "size");
                XGMMLWriter.WriteAttributeString("type", "integer");
                XGMMLWriter.WriteAttributeString("value", Node.Citations.ToString());
                XGMMLWriter.WriteEndElement();
                XGMMLWriter.WriteEndElement();
            }

            //Write edges
            foreach (var Edge in Edges)
            {
                XGMMLWriter.WriteStartElement("edge");
                XGMMLWriter.WriteAttributeString("label", Edge.StartSourceId.ToString() + "-" + Edge.EndSourceId.ToString());
                XGMMLWriter.WriteAttributeString("source", Edge.StartSourceId.ToString());
                XGMMLWriter.WriteAttributeString("target", Edge.EndSourceId.ToString());
                XGMMLWriter.WriteEndElement();
            }

            //Write root element end
            XGMMLWriter.WriteEndElement();
            XGMMLWriter.WriteEndDocument();

            //Cleanup
            XGMMLWriter.Flush();
            XGMMLWriter.Close();
        }
    }
}
