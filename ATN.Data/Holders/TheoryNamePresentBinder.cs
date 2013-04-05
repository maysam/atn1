using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATN.Data
{
    public class TheoryNamePresentBinder
    {
        public long SourceId { get; set; }
        public bool InSubject { get; set; }
        public bool InTitleOrAbstract { get; set; }
        public TheoryNamePresentBinder()
        {

        }
    }

    public class TheoryNamePresentBinderComparer : IEqualityComparer<TheoryNamePresentBinder>
    {
        public bool Equals(TheoryNamePresentBinder x,
            TheoryNamePresentBinder y)
        {
            return x.SourceId == y.SourceId;
        }

        public int GetHashCode(TheoryNamePresentBinder bxx)
        {
            return bxx.GetHashCode();
        }

    }
}
