﻿using System;
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
    }

    /// <summary>
    /// Used for storing data items used on the theory webUI page
    /// </summary>
    public class ExtendedSource : CompleteSource
    {
        public bool metaAnalysis { get; set; }
        public int? contributing { get; set; }
        public double aefScore { get; set; }
        public int depth { get; set; }
    }
}
