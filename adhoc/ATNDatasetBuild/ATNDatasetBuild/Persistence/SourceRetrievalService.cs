using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler.Persistence
{
    public class SourceRetrievalService : DatabaseInterface
    {
        public SourceRetrievalService()
        {

        }

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
