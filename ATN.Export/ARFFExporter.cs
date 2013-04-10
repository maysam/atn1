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
        public static void ExportTrain(SourceNode[] Nodes, int TheoryId, Stream DestinationStream)
        {
            UnicodeEncoding UniEncoding = new UnicodeEncoding();
            AnalysisInterface AnalysisInterface = new AnalysisInterface();
            StreamWriter ExportDestination = new System.IO.StreamWriter(DestinationStream, Encoding.ASCII);

            // Write header to file
            // Depth, IF, AEF computed for Theory 2 as of 04-02-13
            ExportDestination.WriteLine("@RELATION atn");
            ExportDestination.WriteLine("@ATTRIBUTE TheoryId Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE SourceId Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE ArticleLevelEigenFactor Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE ImpactFactor Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE Depth Numeric");
            //ExportDestination.WriteLine("@ATTRIBUTE TheoryAttributionRatio Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE class {contributing, not-contributing}");
            ExportDestination.WriteLine();
            ExportDestination.WriteLine("@DATA");
            ExportDestination.WriteLine();

            //Write training data to file
            foreach (var Node in Nodes)
            {
                TheoryMembership TM = AnalysisInterface.GetTheoryMembershipForSource(Node.SourceId, TheoryId);
                TheoryMembershipSignificance TMS = AnalysisInterface.GetTheoryMembershipSignificanceForSource(Node.SourceId, TheoryId);

                if ((bool)TMS.RAMarkedContributing)
                {
                    ExportDestination.WriteLine("{0}, {1}, {2}, {3}, {4}, contributing",
                            TheoryId.ToString(), TM.SourceId.ToString(), TM.ArticleLevelEigenFactor.ToString(),
                        //TM.TheoryAttributionRatio.ToString());
                            TM.ImpactFactor.ToString(), TM.Depth.ToString()
                        );
                }
                else
                {
                    ExportDestination.WriteLine("{0}, {1}, {2}, {3}, {4}, not-contributing",
                            TheoryId.ToString(), TM.SourceId.ToString(), TM.ArticleLevelEigenFactor.ToString(),
                        //TM.TheoryAttributionRatio.ToString());
                            TM.ImpactFactor.ToString(), TM.Depth.ToString()
                        );
                }
            }
            ExportDestination.Close();
        }

        /// <summary>
        /// Export nodes into an ARFF test data file for J48 classification.
        /// </summary>
        /// <param name="Nodes">Unmarked nodes to be classified</param>
        /// <param name="DestinationStream">Output ARFF test data file</param>
        public static void ExportTest(SourceNode[] Nodes, int TheoryId, Stream DestinationStream)
        {
            UnicodeEncoding UniEncoding = new UnicodeEncoding();
            AnalysisInterface AnalysisInterface = new AnalysisInterface();
            StreamWriter ExportDestination = new System.IO.StreamWriter(DestinationStream, Encoding.ASCII);

            // Write header to file
            ExportDestination.WriteLine("@RELATION atn-test");
            ExportDestination.WriteLine("@ATTRIBUTE TheoryId Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE SourceId Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE ArticleLevelEigenFactor Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE ImpactFactor Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE Depth Numeric");
            //ExportDestination.WriteLine("@ATTRIBUTE TheoryAttributionRatio Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE class {contributing, not-contributing}");
            ExportDestination.WriteLine();
            ExportDestination.WriteLine("@DATA");
            ExportDestination.WriteLine();

            //Write training data to file
            foreach (var Node in Nodes)
            {
                TheoryMembership TM = AnalysisInterface.GetTheoryMembershipForSource(Node.SourceId, TheoryId);

                ExportDestination.WriteLine("{0}, {1}, {2}, {3}, {4}, ?",
                    TheoryId.ToString(), TM.SourceId.ToString(), TM.ArticleLevelEigenFactor.ToString(),
                    //TM.TheoryAttributionRatio.ToString());
                    TM.ImpactFactor.ToString(), TM.Depth.ToString()
                );
            }
            ExportDestination.Close();
        }
    }
}
