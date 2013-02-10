using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATN.Data;

namespace ATN.Data
{
    public class CanonicalDataSource
    {
        public string[] CanonicalIds { get; set; }
        public CrawlerDataSource DataSource { get; set; }
        public CanonicalDataSource(CrawlerDataSource DataSource, params string[] CanonicalIds)
        {
            this.CanonicalIds = CanonicalIds;
            this.DataSource = DataSource;
        }
    }
    public class ExistingCrawlSpecifier : PendingCrawlSpecifier
    {
        public Crawl Crawl { get; set; }
        public ExistingCrawlSpecifier(Crawl Crawl, string TheoryName, int TheoryId, params CanonicalDataSource[] CanonicalDataSourceIds)
            : base(TheoryId, TheoryName, CanonicalDataSourceIds)
        {
            this.Crawl = Crawl;
        }
        public ExistingCrawlSpecifier(Crawl Crawl, string TheoryName, int TheoryId, TheoryDefinition[] CrawlDefinitions) :
            base(TheoryId, TheoryName, CrawlDefinitions)
        {
            this.Crawl = Crawl;
        }
        public ExistingCrawlSpecifier(Crawl Crawl, PendingCrawlSpecifier PendingSpecifier) : base(PendingSpecifier.TheoryId, PendingSpecifier.TheoryName, PendingSpecifier.CanonicalDataSources)
        {
            this.Crawl = Crawl;
        }
    }
    public class PendingCrawlSpecifier : NewCrawlSpecifier
    {
        public int TheoryId { get; set; }
        public PendingCrawlSpecifier(int TheoryId, string TheoryName, params CanonicalDataSource[] CanonicalDataSourceIds)
            : base(TheoryName, CanonicalDataSourceIds)
        {
            this.TheoryId = TheoryId;
        }
        public PendingCrawlSpecifier(int TheoryId, string TheoryName, TheoryDefinition[] CrawlDefinitions)
            : base(TheoryName, CrawlDefinitions)
        {
            this.TheoryId = TheoryId;
        }
        public PendingCrawlSpecifier(int TheoryId, NewCrawlSpecifier NewSpecifier) : base(NewSpecifier.TheoryName, NewSpecifier.CanonicalDataSources)
        {
            this.TheoryId = TheoryId;
        }

    }
    public class NewCrawlSpecifier
    {
        public string TheoryName { get; set; }
        public CanonicalDataSource[] CanonicalDataSources { get; set; }
        public NewCrawlSpecifier(string TheoryName, params CanonicalDataSource[] CanonicalDataSourceIds)
        {
            this.TheoryName = TheoryName;
            this.CanonicalDataSources = CanonicalDataSourceIds;
        }
        public NewCrawlSpecifier(string TheoryName, TheoryDefinition[] CrawlDefinitions)
        {
            this.TheoryName = TheoryName;
            CanonicalDataSources = CrawlDefinitions.Select(cd => new CanonicalDataSource((CrawlerDataSource)cd.DataSourceId, cd.CanonicalIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))).ToArray();
        }
    }
}
