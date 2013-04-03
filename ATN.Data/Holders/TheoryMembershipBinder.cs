using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATN.Data
{
    public class TheoryMembershipBinder
    {
        public int TheoryId { get; set; }
        public long SourceId { get; set; }
        public int RunId { get; set; }
        public short Depth { get; set; }
        public int? ImpactFactor { get; set; }
        public double? TheoryAttributionRatio { get; set; }
        public double? ArticleLevelEigenFactor { get; set; }
        public bool? TheoryNamePresent { get; set; }
        public double? PredictionProbability { get; set; }
        public bool? IsContributingPrediction { get; set; }
        public TheoryMembershipBinder(int TheoryId, long SourceId, int RunId, short Depth, int? ImpactFactor, double? TheoryAttributionRatio,
            double? ArticleLevelEigenFactor, bool? TheoryNamePresent, double? PredictionProbability, bool? IsContributingPrediction)
        {
            this.TheoryId = TheoryId;
            this.SourceId = SourceId;
            this.RunId = RunId;
            this.Depth = Depth;
            this.ImpactFactor = ImpactFactor;
            this.TheoryAttributionRatio = TheoryAttributionRatio;
            this.ArticleLevelEigenFactor = ArticleLevelEigenFactor;
            this.TheoryNamePresent = TheoryNamePresent;
            this.PredictionProbability = PredictionProbability;
            this.IsContributingPrediction = IsContributingPrediction;
        }
    }
}
