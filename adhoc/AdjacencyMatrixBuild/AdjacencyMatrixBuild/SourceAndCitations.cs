using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace AdjacencyMatrixBuild
{
    public class SourceAndCitations
    {
        public Source Source { get; set; }
        public EntityCollection<Source> Citations { get; set; }
    }
}
