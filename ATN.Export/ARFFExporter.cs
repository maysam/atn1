using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ATN.Data;

namespace ATN.Export
{
    public class ARFFExporter
    {
        /// <summary>
        /// Export nodes into an ARFF training data file for analysis in WEKA.
        /// </summary>
        /// <param name="Nodes">Nodes marked by RAs to train decision tree with</param>
        /// <param name="DestinationStream">Output ARFF training data file</param>
        public static void Export(ExtendedSource[] Sources, int TheoryId, Stream DestinationStream)
        {
            StreamWriter ExportDestination = new System.IO.StreamWriter(DestinationStream, Encoding.ASCII);

            // Write header to file
            ExportDestination.WriteLine("@RELATION atn");
            ExportDestination.WriteLine("@ATTRIBUTE Year Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE Depth Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE ImpactFactor Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE ArticleLevelEigenFactor Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE TheoryAttributionRatio Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE class {contributing, not-contributing}");
            ExportDestination.WriteLine();
            ExportDestination.WriteLine("@DATA");
            ExportDestination.WriteLine();

            //Write training data to file
            foreach (var Source in Sources)
            {
                ExportDestination.WriteLine("{0},{1},{2},{3},{4},{5}",
                    Source.Year != 0 ? Source.Year.ToString() : "?", Source.Depth.ToString(), Source.ImpactFactor.ToString(),
                    Source.AEF.HasValue ? Source.AEF.Value.ToString("F20") : "?", Source.TAR.HasValue ? Source.TAR.Value.ToString("F20") : "?",
                    Source.Contributing.HasValue ? (Source.Contributing.Value ? "contributing" : "not-contributing") : "?");
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
            ExportDestination.WriteLine("@ATTRIBUTE SourceId Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE ArticleLevelEigenFactor Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE TheoryAttributionRatio Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE class {contributing, not-contributing}");
            ExportDestination.WriteLine();
            ExportDestination.WriteLine("@DATA");
            ExportDestination.WriteLine();

            //Write training data to file
            foreach (var Node in Nodes)
            {
                TheoryMembership TM = AnalysisInterface.GetTheoryMembershipForSource(Node.SourceId,TheoryId);

                ExportDestination.WriteLine("{0}, {1}, {2}, ?",
                    TM.SourceId.ToString(), TM.ArticleLevelEigenFactor.ToString(), TM.TheoryAttributionRatio.ToString());
            }
            ExportDestination.Close();
        }
    }
}
