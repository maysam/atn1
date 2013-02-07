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
        public SourceEdge(long StartSourceId, long EndSourceId)
        {
            this.StartSourceId = StartSourceId;
            this.EndSourceId = EndSourceId;
        }
    }
}
