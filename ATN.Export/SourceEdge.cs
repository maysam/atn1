using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATN.Export
{
    public class SourceEdge
    {
        public long StartSourceId { get; set; }
        public long EndSourceId { get; set; }
        public int IF { get; set; }
        public SourceEdge(long StartSourceId, long EndSourceId, int IF = 1)
        {
            this.StartSourceId = StartSourceId;
            this.EndSourceId = EndSourceId;
            this.IF = IF;
        }

        public override string ToString()
        {
            return "{" + StartSourceId + " -> " + EndSourceId + "}";
        }
    }
}
