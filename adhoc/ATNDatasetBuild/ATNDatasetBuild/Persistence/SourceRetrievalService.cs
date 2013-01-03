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

        public Source GetSourceByCanonicalId(CrawlerDataSource DataSource, string CanonicalId)
        {
            switch (DataSource)
            {
                case CrawlerDataSource.MicrosoftAcademicSearch:
                    return Context.Sources.Where(s => s.DataSourceId == (int)CrawlerDataSource.MicrosoftAcademicSearch && s.MasID.Value == Int64.Parse(CanonicalId)).SingleOrDefault();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
