using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ATN.Export
{
    class ARFFExporter
    {
        public static void Export(SourceNode[] Nodes, SourceEdge[] Edges, Stream DestinationStream)
        {
            //This method needs to identify training data from the database, generate and export
            //an ARFF file of training data for use by WEKA, and then generate the corresponding test data
            //for making predictions.
        }
    }
}
