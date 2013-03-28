using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATN.Data
{
    /// <summary>
    /// Used for storing data items used on the theory webUI page
    /// </summary>
    public class ExtendedSource : CompleteSource
    {
        public bool metaAnalysis { get; set; }
        public int? numContributing { get; set; }
        public bool? isContributing { get; set; }
        public double? aefScore { get; set; }
        public int depth { get; set; }

        /// <summary>
        /// Construct an extended source from a complete source
        /// </summary>
        /// <param name="cs">The complete source to use data from</param>
        public ExtendedSource(CompleteSource cs) 
        {
            IsDetached = cs.IsDetached;
            Source = cs.Source;
            Authors = cs.Authors;
            Editors = cs.Editors;
            Subjects = cs.Subjects;
            Journal = cs.Journal;
        }
        public ExtendedSource()
        {
            //empty constructor
        }
    }


}
