using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATN.Data
{
    /// <summary>
    /// Used to represent the complete state of a source's latest contribution to a theory
    /// </summary>
    public class CompleteTheoryMembership
    {
        public TheoryMembership TheoryMembership { get; set; }
        public TheoryMembershipSignificance TheoryMembershipSignificance { get; set; }
        public int NumberContributing { get; set; }
        public CompleteTheoryMembership(TheoryMembership TheoryMembership, TheoryMembershipSignificance TheoryMembershipSignificance, int NumberContributing)
        {
            this.TheoryMembership = TheoryMembership;
            this.TheoryMembershipSignificance = TheoryMembershipSignificance;
            this.NumberContributing = NumberContributing;
        }
    }
}
