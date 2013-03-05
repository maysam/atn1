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
            StreamWriter ExportDestination = new System.IO.StreamWriter(DestinationStream, Encoding.ASCII);

            // Write header to file
            ExportDestination.WriteLine("@RELATION atn");
            ExportDestination.WriteLine("@ATTRIBUTE SourceId Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE EigenFactorValue Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE AttentionRatio Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE class {contributing, not-contributing}");
            ExportDestination.WriteLine();
            ExportDestination.WriteLine("@DATA");
            ExportDestination.WriteLine();

            //Write training data to file
            foreach (var Node in Nodes)
            {
                TheoryMembership TM = Theories.GetTheoryMembershipForSource(Node.SourceId);
                TheoryMembershipSignificance TMS = Theories.GetTheoryMembershipSignificanceForSource(Node.SourceId);

                if ((bool)TMS.RAMarkedContributing)
                {
                    ExportDestination.WriteLine("{0}, {1}, {2}, contributing",
                        TM.SourceId.ToString(), TM.EigenFactorValue.ToString(), TM.AttentionRatio.ToString());
                }
                else
                {
                    ExportDestination.WriteLine("{0}, {1}, {2}, not-contributing",
                        TM.SourceId.ToString(), TM.EigenFactorValue.ToString(), TM.AttentionRatio.ToString());
                }
            }
            ExportDestination.Close();
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
            StreamWriter ExportDestination = new System.IO.StreamWriter(DestinationStream, Encoding.ASCII);

            // Write header to file
            ExportDestination.WriteLine("@RELATION atn-test");
            ExportDestination.WriteLine("@ATTRIBUTE SourceId Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE EigenFactorValue Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE AttentionRatio Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE class {contributing, not-contributing}");
            ExportDestination.WriteLine();
            ExportDestination.WriteLine("@DATA");
            ExportDestination.WriteLine();

            //Write training data to file
            foreach (var Node in Nodes)
            {
                TheoryMembership TM = Theories.GetTheoryMembershipForSource(Node.SourceId);

                ExportDestination.WriteLine("{0}, {1}, {2}, ?",
                    TM.SourceId.ToString(), TM.EigenFactorValue.ToString(), TM.AttentionRatio.ToString());
            }
            ExportDestination.Close();
        }
    }
}
