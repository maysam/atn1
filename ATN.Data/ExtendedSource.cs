using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATN.Data
{
    /// <summary>
    /// Used for storing data items used on the theory webUI page
    /// </summary>
    public class ExtendedSource : CompleteSource
    {
        public bool metaAnalysis { get; set; }
        public int? numContributing { get; set; }
        public bool? isContributing { get; set; }
        public double aefScore { get; set; }
        public int depth { get; set; }
    }
}
