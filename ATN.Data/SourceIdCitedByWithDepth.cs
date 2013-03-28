using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATN.Data
{
    public class SourceIdCitedByWithDepth
    {
        public long SourceId { get; set; }
        public long? CitesSourceId { get; set; }
        public short Depth { get; set; }
        public SourceIdCitedByWithDepth()
        {

        }
        public SourceIdCitedByWithDepth(long SourceId, long? CitesSourceId, short Depth)
        {
            this.SourceId = SourceId;
            this.CitesSourceId = CitesSourceId;
            this.Depth = Depth;
        }
    }
}
