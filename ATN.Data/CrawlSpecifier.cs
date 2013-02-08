using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATN.Data;

namespace ATN.Data
{
    public class ExistingCrawlSpecifier : PendingCrawlSpecifier
    {
        public Crawl Crawl { get; set; }
        public ExistingCrawlSpecifier(Crawl Crawl, string TheoryName, int TheoryId, CrawlerDataSource DataSource, params string[][] CanonicalDataSourceIds)
            : base(TheoryId, TheoryName, DataSource, CanonicalDataSourceIds)
        {
            this.Crawl = Crawl;
        }
        public ExistingCrawlSpecifier(Crawl Crawl, string TheoryName, int TheoryId, CrawlerDataSource DataSource, TheoryDefinition[] CrawlDefinitions) :
            base(TheoryId, TheoryName, DataSource, CrawlDefinitions)
        {
            this.Crawl = Crawl;
        }
        public ExistingCrawlSpecifier(Crawl Crawl, PendingCrawlSpecifier PendingSpecifier) : base(PendingSpecifier.TheoryId, PendingSpecifier.TheoryName, PendingSpecifier.DataSource, PendingSpecifier.CanonicalDataSourceIds)
        {
            this.Crawl = Crawl;
        }
    }
    public class PendingCrawlSpecifier : NewCrawlSpecifier
    {
        public int TheoryId { get; set; }
        public PendingCrawlSpecifier(int TheoryId, string TheoryName, CrawlerDataSource DataSource, params string[][] CanonicalDataSourceIds)
            : base(TheoryName, DataSource, CanonicalDataSourceIds)
        {
            this.TheoryId = TheoryId;
        }
        public PendingCrawlSpecifier(int TheoryId, string TheoryName, CrawlerDataSource DataSource, TheoryDefinition[] CrawlDefinitions)
            : base(TheoryName, DataSource, CrawlDefinitions)
        {
            this.TheoryId = TheoryId;
        }
        public PendingCrawlSpecifier(int TheoryId, NewCrawlSpecifier NewSpecifier) : base(NewSpecifier.TheoryName, NewSpecifier.DataSource, NewSpecifier.CanonicalDataSourceIds)
        {
            this.TheoryId = TheoryId;
        }

    }
    public class NewCrawlSpecifier
    {
        public string TheoryName { get; set; }
        public CrawlerDataSource DataSource { get; set; }
        public string[][] CanonicalDataSourceIds { get; set; }
        public NewCrawlSpecifier(string TheoryName, CrawlerDataSource DataSource, params string[][] CanonicalDataSourceIds)
        {
            this.TheoryName = TheoryName;
            this.DataSource = DataSource;
            this.CanonicalDataSourceIds = CanonicalDataSourceIds;
        }
        public NewCrawlSpecifier(string TheoryName, CrawlerDataSource DataSource, TheoryDefinition[] CrawlDefinitions)
        {
            this.TheoryName = TheoryName;
            this.DataSource = DataSource;
            CanonicalDataSourceIds = CrawlDefinitions.Select(cd => new string[] { cd.CanonicalIds }).ToArray();
        }
    }
}
