using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATN.Data
{
    /// <summary>
    /// Used to store a representation of a source, the source it cites, and the depth of citation
    /// This class merely exists as a binder for retrieving citation information using raw TSQL.
    /// </summary>
    public class SourceIdCitedByWithDepth
    {
        public long SourceId { get; set; }
        public long? CitesSourceId { get; set; }
        public short Depth { get; set; }
        public int ImpactFactor { get; set; }
    }

    public class TheoryMembershipSignificanceBinder
    {
        public int TheoryId { get; set; }
        public long SouceId { get; set; }
        public bool? RAMarkedContributing { get; set; }
        public bool IsMetaAnalysis { get; set; }
        public TheoryMembershipSignificanceBinder(int TheoryId, long SourceId, bool? RAMarkedContributing, bool IsMetaAnalysis)
        {
            this.TheoryId = TheoryId;
            this.SouceId = SourceId;
            this.RAMarkedContributing = RAMarkedContributing;
            this.IsMetaAnalysis = IsMetaAnalysis;
        }
    }

    public class TheoryInitialiationBinder
    {
        public int TheoryId { get; set; }
        public long SouceId { get; set; }
        public int RunId { get; set; }
        public short Depth { get; set; }
        public int ImpactFactor { get; set; }
        public TheoryInitialiationBinder(int TheoryId, long SourceId, int RunId, short Depth, int ImpactFactor)
        {
            this.TheoryId = TheoryId;
            this.SouceId = SourceId;
            this.RunId = RunId;
            this.Depth = Depth;
            this.ImpactFactor = ImpactFactor;
        }
    }
}
