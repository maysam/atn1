using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATN.Data
{
    /// <summary>
    /// Used for storing data items used on the theory webUI page
    /// </summary>
    public class ExtendedSource : ExportSource
    {
        public int? ImpactFactor { get; set; }
        public bool IsMetaAnalysis { get; set; }
        public int? NumContributing { get; set; }
        public bool? Contributing { get; set; }
        public double? AEF { get; set; }
        public double? TAR { get; set; }
        public double? predictionProbability { get; set; }
        public bool? isContributingPrediction { get; set; }
        public bool TheoryNamePresent { get; set; }
        public short Depth { get; set; }

        public ExtendedSource()
        {
            //empty constructor
        }
    }
}
