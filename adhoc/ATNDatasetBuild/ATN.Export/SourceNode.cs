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
        public int Citations { get; set; }
        public SourceNode(long SourceId, string Title, int Citations)
        {
            this.SourceId = SourceId;
            this.Title = Title;
            this.Citations = Citations;
        }
    }
}
