using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATN.Data
{
    /// <summary>
    /// Represents a source, its references, and its theory contribution
    /// </summary>
    public class SourceWithReferences
    {
        public int TheoryId {get; set;}
        public long SourceId { get; set; }
        public int? ImpactFactor { get; set; }
        public int OutFactor
        {
            get
            {
                return References.Count;
            }
        }
        public short Depth { get; set; }
        public double? TheoryAttributionRatio { get; set; }
        public double? ArticleLevelEigenFactor { get; set; }
        public double? PredictionProbability { get; set; }
        public bool? IsContributingPrediction { get; set; }
        public List<long> References { get; set; }
        public List<long> Citations { get; set; }
        public int? Year { get; set; }
        public SourceWithReferences(long SourceId, int ImpactFactor, short Depth)
        {
            this.Citations = new List<long>();
            this.References = new List<long>(50);
            this.SourceId = SourceId;
            this.ImpactFactor = ImpactFactor;
            this.Depth = Depth;
        }
        public void AddCitation(long SourceId)
        {
            this.Citations.Add(SourceId);
        }
        public void AddReference(long SourceId)
        {
            this.References.Add(SourceId);
        }
    }
}
