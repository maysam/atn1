using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATN.Data
{
    /// <summary>
    /// Used for storing a complete record of a source, in addition to whether the given data-model objects are actively attached to the persistence model
    /// </summary>
    public class CompleteSource
    {
        /// <summary>
        /// Indicates whether this instance is actively attached to the persistence model
        /// </summary>
        public bool IsDetached { get; set; }
        public Source Source {get; set;}
        public Author[] Authors {get; set;}
        public Editor[] Editors { get; set; }
        public Subject[] Subjects { get; set; }
        public Journal Journal { get; set; }
        public CompleteSource()
        {

        }
        public CompleteSource(Source Source, Author[] Authors, Journal Journal)
        {
            this.Source = Source;
            this.Authors = Authors;
            this.Journal = Journal;
        }
    }
}
