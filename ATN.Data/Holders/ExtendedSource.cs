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
        public bool isMetaAnalysis { get; set; }
        public int? numContributing { get; set; }
        public bool? isContributing { get; set; }
        public double? aef { get; set; }
        public double? tar { get; set; }
        public double? predictionProbability { get; set; }
        public bool? isContributingPrediction { get; set; }
        public short depth { get; set; }

        public ExtendedSource()
        {
            //empty constructor
        }
    }
}
