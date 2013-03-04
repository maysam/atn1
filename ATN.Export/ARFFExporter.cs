using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ATN.Data;

namespace ATN.Export
{
    class ARFFExporter
    {
        /// <summary>
        /// Export nodes into an ARFF training data file for analysis in WEKA.
        /// </summary>
        /// <param name="Nodes">Nodes marked by RAs to train decision tree with</param>
        /// <param name="DestinationStream">Output ARFF training data file</param>
        public static void ExportTrain(SourceNode[] Nodes, Stream DestinationStream)
        {
            UnicodeEncoding UniEncoding = new UnicodeEncoding();
            Theories Theories = new Theories();

            // Write header to file
            string ARFFHeader = "@RELATION atn\n";
            ARFFHeader += "@ATTRIBUTE SourceId Numeric\n";
            ARFFHeader += "@ATTRIBUTE EigenFactorValue Numeric\n";
            ARFFHeader += "@ATTRIBUTE AttentionRatio Numeric\n";
            ARFFHeader += "@ATTRIBUTE class {contributing, not-contributing}\n\n";
            ARFFHeader += "@DATA\n\n";
            DestinationStream.Write(UniEncoding.GetBytes(ARFFHeader),0,UniEncoding.GetByteCount(ARFFHeader));

            //Write training data to file
            string datum = null;
            foreach (var Node in Nodes)
            {
                TheoryMembership TM = Theories.GetTheoryMembershipForSource(Node.SourceId);
                TheoryMembershipSignificance TMS = Theories.GetTheoryMembershipSignificanceForSource(Node.SourceId);
                datum = "";

                //SourceId
                datum += TM.SourceId.ToString();
                datum += ", ";
                //EigenFactorValue
                datum += TM.EigenFactorValue.ToString();
                datum += ", ";
                //AttentionRatio
                datum += TM.AttentionRatio.ToString();
                datum += ", ";
                //class
                if ((bool)TMS.RAMarkedContributing)
                {
                    datum += "contributing\n";
                }
                else
                {
                    datum += "not-contributing\n";
                }

                DestinationStream.Write(UniEncoding.GetBytes(datum), 0, UniEncoding.GetByteCount(datum));
            }

            DestinationStream.Flush();
            DestinationStream.Close();
        }

        /// <summary>
        /// Export nodes into an ARFF test data file for J48 classification.
        /// </summary>
        /// <param name="Nodes">Unmarked nodes to be classified</param>
        /// <param name="DestinationStream">Output ARFF test data file</param>
        public static void ExportTest(SourceNode[] Nodes, Stream DestinationStream)
        {
            UnicodeEncoding UniEncoding = new UnicodeEncoding();
            Theories Theories = new Theories();

            // Write header to file
            string ARFFHeader = "@RELATION atn-test\n";
            ARFFHeader += "@ATTRIBUTE SourceId Numeric\n";
            ARFFHeader += "@ATTRIBUTE EigenFactorValue Numeric\n";
            ARFFHeader += "@ATTRIBUTE AttentionRatio Numeric\n";
            ARFFHeader += "@ATTRIBUTE class {contributing, not-contributing}\n\n";
            ARFFHeader += "@DATA\n\n";
            DestinationStream.Write(UniEncoding.GetBytes(ARFFHeader), 0, UniEncoding.GetByteCount(ARFFHeader));

            //Write training data to file
            string datum = null;
            foreach (var Node in Nodes)
            {
                TheoryMembership TM = Theories.GetTheoryMembershipForSource(Node.SourceId);
                datum = "";

                //SourceId
                datum += TM.SourceId.ToString();
                datum += ", ";
                //EigenFactorValue
                datum += TM.EigenFactorValue.ToString();
                datum += ", ";
                //AttentionRatio
                datum += TM.AttentionRatio.ToString();
                datum += ", ";
                //class
                datum += "?\n";

                DestinationStream.Write(UniEncoding.GetBytes(datum), 0, UniEncoding.GetByteCount(datum));
            }

            DestinationStream.Flush();
            DestinationStream.Close();
        }
    }
}
