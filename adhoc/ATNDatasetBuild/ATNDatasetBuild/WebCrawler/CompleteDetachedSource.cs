using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crawler.Persistence;

namespace Crawler.WebCrawler
{
    /// <summary>
    /// Used for storing a complete record of a source, in addition to whether the given data-model objects are actively attached to the persistence model
    /// </summary>
    public class CompleteSource
    {
        public bool IsDetached { get; set; }
        public Source Source {get; set;}
        public Author[] Authors {get; set;}
        public Editor[] Editors { get; set; }
        public Journal Journal { get; set; }
    }
}
