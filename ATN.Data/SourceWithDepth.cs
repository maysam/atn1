using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATN.Data
{
    public class SourceIdWithDepth
    {
        public long SourceId { get; set; }
        public short Depth { get; set; }
        public int ImpactFactor { get; set; }
        public SourceIdWithDepth(long SourceId, short Depth)
        {
            this.SourceId = SourceId;
            this.Depth = Depth;
        }
        public SourceIdWithDepth(long SourceId, short Depth, int ImpactFactor)
        {
            this.SourceId = SourceId;
            this.Depth = Depth;
            this.ImpactFactor = ImpactFactor;
        }
    }
}
