using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler.Persistence
{
    /// <summary>
    /// A service for retrieving a unique Source object based on the passed data source and data-source specific identifier
    /// </summary>
    public class SourceRetrievalService : DatabaseInterface
    {
        
        public SourceRetrievalService()
        {

        }

        /// <summary>
        /// Retrieves the unique Source corresponding to the given data source and passed data-source specific identifier
        /// </summary>
        /// <param name="DataSource">The data-source from which to retrieve</param>
        /// <param name="DataSourceSpecificId">The data-source specific identifier for which to retrieve</param>
        /// <returns>A unique Source corresponding to the given data source and data-source specific identifier</returns>
        public Source GetSourceByDataSourceSpecificId(CrawlerDataSource DataSource, string DataSourceSpecificId)
        {
            switch (DataSource)
            {
                case CrawlerDataSource.MicrosoftAcademicSearch:
                    long SourceId =  Int64.Parse(DataSourceSpecificId);
                    return Context.Sources.Where(s => s.DataSourceId == (int)CrawlerDataSource.MicrosoftAcademicSearch && s.MasID.Value == SourceId).SingleOrDefault();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
