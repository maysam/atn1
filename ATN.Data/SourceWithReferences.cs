using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATN.Data
{
    /// <summary>
    /// Represents a source, it's impact for a given theory, it's depth, and its references
    /// </summary>
    public class SourceWithReferences
    {
        public long SourceId { get; set; }
        public int ImpactFactor { get; set; }
        public int OutFactor
        {
            get
            {
                return References.Count;
            }
        }
        public short Depth { get; set; }
        public List<long> References { get; set; }
        public SourceWithReferences(long SourceId, int ImpactFactor, short Depth)
        {
            this.References = new List<long>();
            this.SourceId = SourceId;
            this.ImpactFactor = ImpactFactor;
            this.Depth = Depth;
        }
        public void AddReference(long SourceId)
        {
            this.References.Add(SourceId);
        }
    }
}
