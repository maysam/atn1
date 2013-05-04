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
        public static void Export(ExtendedSource[] Sources, int TheoryId, Stream DestinationStream, bool Close = true)
        {
            StreamWriter ExportDestination = new System.IO.StreamWriter(DestinationStream, Encoding.ASCII);

            // Write header to file
            // Depth, IF, AEF computed for Theory 2 as of 04-02-13
            ExportDestination.WriteLine("@RELATION atn");
            ExportDestination.WriteLine("@ATTRIBUTE Year Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE Depth Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE ImpactFactor Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE ArticleLevelEigenFactor Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE TheoryAttributionRatio Numeric");
            ExportDestination.WriteLine("@ATTRIBUTE TheoryNamePresent {present, not-present}");
            ExportDestination.WriteLine("@ATTRIBUTE class {contributing, not-contributing}");
            ExportDestination.WriteLine();
            ExportDestination.WriteLine("@DATA");
            ExportDestination.WriteLine();

            //Write training data to file
            foreach (var Source in Sources)
            {
                //ExportDestination.WriteLine("{0},{1},{2},{3},{4},{5},{6}",
                //    Source.SourceId,
                //    Source.Year != 0 ? Source.Year.ToString() : "?", Source.Depth.ToString(), Source.ImpactFactor.HasValue ? Source.ImpactFactor.Value.ToString() : "?",
                //    Source.AEF.HasValue ? Source.AEF.Value.ToString("F20") : "?", Source.TAR.HasValue ? Source.TAR.Value.ToString("F20") : "?",
                //    Source.Contributing.HasValue ? (Source.Contributing.Value ? "contributing" : "not-contributing") : "?");
                ExportDestination.WriteLine("{0},{1},{2},{3},{4},{5},{6}",
                    Source.Year != 0 ? Source.Year.ToString() : "?",
                    Source.Depth.ToString(),
                    Source.ImpactFactor.HasValue ? Source.ImpactFactor.Value.ToString() : "?",
                    Source.AEF.HasValue ? Source.AEF.Value.ToString("F20") : "?",
                    "?", //Source.TAR.HasValue ? Source.TAR.Value.ToString("F20") : "?",
                    Source.TheoryNamePresent ? "present" : "not-present",
                    Source.Contributing.HasValue ? (Source.Contributing.Value ? "contributing" : "not-contributing") : "?");
            }

            ExportDestination.Flush();

            if (Close)
            {
                ExportDestination.Close();
            }
        }
    }
}
