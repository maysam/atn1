using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATN.Data;

namespace ATN.Data
{
    /// <summary>
    /// Represents a list of canonical identifiers and their target data source
    /// </summary>
    public class CanonicalDataSource
    {
        /// <summary>
        /// The list of canonical identifiers
        /// </summary>
        public string[] CanonicalIds { get; set; }

        /// <summary>
        /// The data source the canonical identifiers correspond to
        /// </summary>
        public CrawlerDataSource DataSource { get; set; }

        /// <summary>
        /// Creates the representation of canonical identifiers and their data source
        /// </summary>
        /// <param name="DataSource">The data source the canonical identifiers correspond to</param>
        /// <param name="CanonicalIds">The list of canonical identifiers</param>
        public CanonicalDataSource(CrawlerDataSource DataSource, params string[] CanonicalIds)
        {
            this.CanonicalIds = CanonicalIds;
            this.DataSource = DataSource;
        }
    }

    /// <summary>
    /// Represents an existing crawl
    /// </summary>
    public class ExistingCrawlSpecifier : PendingCrawlSpecifier
    {
        /// <summary>
        /// The Crawl this object corresponds to
        /// </summary>
        public Crawl Crawl { get; set; }

        /// <summary>
        /// Creates the Crawl representation
        /// </summary>
        /// <param name="Crawl">The Crawl this object corresponds to</param>
        /// <param name="TheoryName">The name of the theory this crawl is for</param>
        /// <param name="TheoryComment">Any additional information regarding the theory</param>
        /// <param name="TheoryId">The identifier for the named theory</param>
        /// <param name="AEF">Whether AEF should be calculated for this theory. Requires TAR be computed.</param>
        /// <param name="Clustering">Whether Clustering should be run on this theory</param>
        /// <param name="DataMining">Whether data mining should be run on this theory</param>
        /// <param name="ImpactFactor">Whether ImpactFactor should be calculated for this theory</param>
        /// <param name="TAR">Whether theory attribution ratio should be calculated for this theory. Requires AEF and ImpactFactor be computed.</param>
        /// <param name="CanonicalDataSourceIds">A representation of the canonical data-source specific sources for the given theory</param>
        public ExistingCrawlSpecifier(Crawl Crawl, string TheoryName, string TheoryComment, int TheoryId, bool AEF, bool ImpactFactor, bool TAR, bool DataMining, bool Clustering, params CanonicalDataSource[] CanonicalDataSourceIds)
            : base(TheoryId, TheoryName, TheoryComment, Crawl.CrawlIntervalDays, AEF, ImpactFactor, TAR, DataMining, Clustering, CanonicalDataSourceIds)
        {
            this.Crawl = Crawl;
        }

        /// <summary>
        /// Creates the Crawl representation, deriving the canonical data sources from theory definitions
        /// </summary>
        /// <param name="Crawl">The Crawl this object corresponds to</param>
        /// <param name="TheoryName">The name of the theory this crawl is for</param>
        /// <param name="TheoryComment">Any additional information regarding the theory</param>
        /// <param name="TheoryId">The identifier for the named theory</param>
        /// <param name="AEF">Whether AEF should be calculated for this theory. Requires TAR be computed.</param>
        /// <param name="Clustering">Whether Clustering should be run on this theory</param>
        /// <param name="DataMining">Whether data mining should be run on this theory</param>
        /// <param name="ImpactFactor">Whether ImpactFactor should be calculated for this theory</param>
        /// <param name="TAR">Whether theory attribution ratio should be calculated for this theory. Requires AEF and ImpactFactor be computed.</param>
        /// <param name="CrawlDefinitions">An existing set of theory definitions for the named theory</param>
        public ExistingCrawlSpecifier(Crawl Crawl, string TheoryName, string TheoryComment, int TheoryId, bool AEF, bool ImpactFactor, bool TAR, bool DataMining, bool Clustering, TheoryDefinition[] CrawlDefinitions) :
            base(TheoryId, TheoryName, TheoryComment, Crawl.CrawlIntervalDays, AEF, ImpactFactor, TAR, DataMining, Clustering, CrawlDefinitions)
        {
            this.Crawl = Crawl;
        }

        /// <summary>
        /// Creates the Crawl representation using an existing PendingCrawlSpecifier
        /// </summary>
        /// <param name="Crawl">The Crawl this object corresponds to</param>
        /// <param name="PendingSpecifier">An existing PendingCrawlSpecifier from which to derive</param>
        public ExistingCrawlSpecifier(Crawl Crawl, PendingCrawlSpecifier PendingSpecifier) : base(PendingSpecifier.TheoryId, PendingSpecifier.TheoryName, PendingSpecifier.TheoryComment, PendingSpecifier.CrawlIntervalDays, PendingSpecifier.AEF, PendingSpecifier.ImpactFactor, PendingSpecifier.TAR, PendingSpecifier.DataMining, PendingSpecifier.Clustering, PendingSpecifier.CanonicalDataSources)
        {
            this.Crawl = Crawl;
        }
    }

    /// <summary>
    /// Represents a new crawl which has not been added to the persistence model, while the corresponding theory has
    /// </summary>
    public class PendingCrawlSpecifier : NewCrawlSpecifier
    {
        /// <summary>
        /// The identifier for the named theory
        /// </summary>
        public int TheoryId { get; set; }

        /// <summary>
        /// The interval, in days, between refreshes of the named theory or null if the theory will not be automatically refreshed
        /// </summary>
        public int? CrawlIntervalDays { get; set; }

        /// <summary>
        /// Creates a crawl representation corresponding with a crawl which has had its theory committed to the persistence-model
        /// </summary>
        /// <param name="TheoryId">The identifier for the named theory</param>
        /// <param name="TheoryName">The name of the theory this crawl is for</param>
        /// <param name="AEF">Whether AEF should be calculated for this theory. Requires TAR be computed.</param>
        /// <param name="Clustering">Whether Clustering should be run on this theory</param>
        /// <param name="DataMining">Whether data mining should be run on this theory</param>
        /// <param name="ImpactFactor">Whether ImpactFactor should be calculated for this theory</param>
        /// <param name="TAR">Whether theory attribution ratio should be calculated for this theory. Requires AEF and ImpactFactor be computed.</param>
        /// <param name="CrawlIntervalDays">The interval, in days, between refreshes of the named theory or null if the theory will not be automatically refreshed</param>
        /// <param name="CanonicalDataSourceIds">A representation of the canonical data-source specific sources for the given theory</param>
        public PendingCrawlSpecifier(int TheoryId, string TheoryName, string TheoryComment, int? CrawlIntervalDays, bool AEF, bool ImpactFactor, bool TAR, bool DataMining, bool Clustering, params CanonicalDataSource[] CanonicalDataSourceIds)
            : base(TheoryName, TheoryComment, AEF, ImpactFactor, TAR, DataMining, Clustering, CanonicalDataSourceIds)
        {
            this.TheoryId = TheoryId;
            this.CrawlIntervalDays = CrawlIntervalDays;
        }

        /// <summary>
        /// Creates a crawl representation corresponding with a crawl which has had its theory committed to the persistence-model; provided for the use of deriving classes
        /// </summary>
        /// <param name="TheoryId">The identifier for the named theory</param>
        /// <param name="TheoryName">The name of the theory this crawl is for</param>
        /// <param name="TheoryComment">Any additional information regarding the theory</param>
        /// <param name="AEF">Whether AEF should be calculated for this theory. Requires TAR be computed.</param>
        /// <param name="Clustering">Whether Clustering should be run on this theory</param>
        /// <param name="DataMining">Whether data mining should be run on this theory</param>
        /// <param name="ImpactFactor">Whether ImpactFactor should be calculated for this theory</param>
        /// <param name="TAR">Whether theory attribution ratio should be calculated for this theory. Requires AEF and ImpactFactor be computed.</param>
        /// <param name="CrawlIntervalDays">The interval, in days, between refreshes of the named theory or null if the theory will not be automatically refreshed</param>
        /// <param name="TheoryDefinitions">An existing set of theory definitions for the named theory</param>
        protected PendingCrawlSpecifier(int TheoryId, string TheoryName, string TheoryComment, int? CrawlIntervalDays, bool AEF, bool ImpactFactor, bool TAR, bool DataMining, bool Clustering, TheoryDefinition[] TheoryDefinitions)
            : base(TheoryName, TheoryComment, AEF, ImpactFactor, TAR, DataMining, Clustering, TheoryDefinitions)
        {
            this.TheoryId = TheoryId;
            this.CrawlIntervalDays = CrawlIntervalDays;
        }

        /// <summary>
        /// Creates a crawl representation corresponding with a crawl which has had its theory committed to the persistence-model using an existing NewCrawlSpecifier
        /// </summary>
        /// <param name="TheoryId">The identifier for the named theory</param>
        /// <param name="NewSpecifier">An existing NewCrawlSpecifier from which to derive</param>
        /// <param name="CrawlIntervalDays">The interval, in days, between refreshes of the named theory or null if the theory will not be automatically refreshed</param>
        public PendingCrawlSpecifier(int TheoryId, NewCrawlSpecifier NewSpecifier, int? CrawlIntervalDays, bool AEF, bool ImpactFactor, bool TAR, bool DataMining, bool Clustering)
            : base(NewSpecifier.TheoryName, NewSpecifier.TheoryComment, AEF, ImpactFactor, TAR, DataMining, Clustering, NewSpecifier.CanonicalDataSources)
        {
            this.TheoryId = TheoryId;
            this.CrawlIntervalDays = CrawlIntervalDays;
        }
    }

    /// <summary>
    /// Represents a new crawl which has not been added to the persistence-model
    /// </summary>
    public class NewCrawlSpecifier
    {
        /// <summary>
        /// The name of the theory this crawl is for
        /// </summary>
        public string TheoryName { get; set; }

        /// <summary>
        /// Any additional information regarding the theory
        /// </summary>
        public string TheoryComment { get; set; }

        /// <summary>
        /// Whether Article-Level EigenFactor should be computed for this theory
        /// </summary>
        public bool AEF { get; set; }

        /// <summary>
        /// Whether ImpactFactor should be computed for this theory
        /// </summary>
        public bool ImpactFactor { get; set; }

        /// <summary>
        /// Whether Theory Attribution Ratio should be computed for this theory
        /// </summary>
        public bool TAR { get; set; }

        /// <summary>
        /// Whether data mining should be run on the theory
        /// </summary>
        public bool DataMining { get; set; }

        /// <summary>
        /// Whether Clustering should be run on the theory
        /// </summary>
        public bool Clustering { get; set; }

        /// <summary>
        /// The canonical data-source specific sources for the given theory
        /// </summary>
        public CanonicalDataSource[] CanonicalDataSources { get; set; }

        /// <summary>
        /// Creates the crawl representation
        /// </summary>
        /// <param name="TheoryName">The name of the theory this crawl is for</param>
        /// <param name="TheoryComment">Any additional information regarding the theory</param>
        /// <param name="AEF">Whether AEF should be calculated for this theory. Requires TAR be computed.</param>
        /// <param name="Clustering">Whether Clustering should be run on this theory</param>
        /// <param name="DataMining">Whether data mining should be run on this theory</param>
        /// <param name="ImpactFactor">Whether ImpactFactor should be calculated for this theory</param>
        /// <param name="TAR">Whether theory attribution ratio should be calculated for this theory. Requires AEF and ImpactFactor be computed.</param>
        /// <param name="CanonicalDataSourceIds">A representation of the canonical data-source specific sources for the given theory</param>
        public NewCrawlSpecifier(string TheoryName, string TheoryComment, bool AEF, bool ImpactFactor, bool TAR, bool DataMining, bool Clustering, params CanonicalDataSource[] CanonicalDataSourceIds)
        {
            this.TheoryName = TheoryName;
            this.CanonicalDataSources = CanonicalDataSourceIds;
            this.TheoryComment = TheoryComment;
            this.AEF = AEF;
            this.ImpactFactor = ImpactFactor;
            this.TAR = TAR;
            this.DataMining = DataMining;
            this.Clustering = Clustering;
        }

        /// <summary>
        /// Creates the crawl representation; provided for the use of deriving classes
        /// </summary>
        /// <param name="TheoryName">The name of the theory this crawl is for</param>
        /// <param name="TheoryComment">Any additional information regarding the theory</param>
        /// <param name="AEF">Whether AEF should be calculated for this theory. Requires TAR be computed.</param>
        /// <param name="Clustering">Whether Clustering should be run on this theory</param>
        /// <param name="DataMining">Whether data mining should be run on this theory</param>
        /// <param name="ImpactFactor">Whether ImpactFactor should be calculated for this theory</param>
        /// <param name="TAR">Whether theory attribution ratio should be calculated for this theory. Requires AEF and ImpactFactor be computed.</param>
        /// <param name="TheoryDefinitions">An existing set of theory definitions for the named theory</param>
        protected NewCrawlSpecifier(string TheoryName,  string TheoryComment, bool AEF, bool ImpactFactor, bool TAR, bool DataMining, bool Clustering, TheoryDefinition[] TheoryDefinitions)
        {
            this.TheoryName = TheoryName;
            this.TheoryComment = TheoryComment;
            this.ImpactFactor = ImpactFactor;
            this.DataMining = DataMining;
            this.Clustering = Clustering;
            this.TAR = TAR;
            CanonicalDataSources = TheoryDefinitions.Select(cd => new CanonicalDataSource((CrawlerDataSource)cd.DataSourceId, cd.CanonicalIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))).ToArray();
        }
    }
}
