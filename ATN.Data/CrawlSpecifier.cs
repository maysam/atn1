using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATN.Data;

namespace ATN.Data
{
    public class ExistingCrawlSpecifier : CrawlSpecifier
    {
        public Crawl Crawl { get; set; }
        public ExistingCrawlSpecifier(Crawl Crawl, int TheoryId, CrawlerDataSource DataSource, params string[][] CanonicalDataSourceIds)
            : base(TheoryId, DataSource, CanonicalDataSourceIds)
        {
            this.Crawl = Crawl;
        }
        public ExistingCrawlSpecifier(Crawl Crawl, int TheoryId, CrawlerDataSource DataSource, TheoryDefinition[] CrawlDefinitions) :
            base(TheoryId, DataSource, CrawlDefinitions)
        {
            this.Crawl = Crawl;
        }
    }
    public class CrawlSpecifier
    {
        public int TheoryId { get; set; }
        public CrawlerDataSource DataSource { get; set; }
        public string[][] CanonicalDataSourceIds { get; set; }
        public CrawlSpecifier(int TheoryId, CrawlerDataSource DataSource, params string[][] CanonicalDataSourceIds)
        {
            this.TheoryId = TheoryId;
            this.DataSource = DataSource;
            this.CanonicalDataSourceIds = CanonicalDataSourceIds;
        }
        public CrawlSpecifier(int TheoryId, CrawlerDataSource DataSource, TheoryDefinition[] CrawlDefinitions)
        {
            this.TheoryId = TheoryId;
            this.DataSource = DataSource;
            CanonicalDataSourceIds = CrawlDefinitions.Select(cd => new string[] { cd.CanonicalIds }).ToArray();
        }
    }
}
