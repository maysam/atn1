using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATN.Export
{
    public class SourceNode
    {
        public long SourceId { get; set; }
        public string Title { get; set; }
        public int? ImpactFactor { get; set; }
        public int Year { get; set; }
        public double? AEF { get; set; }
        public double? TAR { get; set; }
        public double? PredictionProbability { get; set; }
        public bool? IsContributingPrediction;
        public short Depth {get; set;}
        public SourceNode(long SourceId, string Title, int? ImpactFactor, int Year, double? AEF, double? TAR, double? PredictionProbability, bool? IsContributingPrediction, short Depth)
        {
            this.SourceId = SourceId;
            this.Title = Title;
            this.ImpactFactor = ImpactFactor;
            this.Year = Year;
            this.AEF = AEF;
            this.TAR = TAR;
            this.PredictionProbability = PredictionProbability;
            this.IsContributingPrediction = IsContributingPrediction;
            this.Depth = Depth;
        }
    }
}
