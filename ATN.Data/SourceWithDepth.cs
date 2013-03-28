using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATN.Data
{
    public class SourceWithDepth
    {
        public Source Source { get; set; }
        public short Depth { get; set; }
        public int ImpactFactor { get; set; }
        public SourceWithDepth(Source Source, short Depth)
        {
            this.Source = Source;
            this.Depth = Depth;
        }
    }
}
