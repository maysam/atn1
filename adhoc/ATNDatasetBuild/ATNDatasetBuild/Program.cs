using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crawler.MAS;
using Crawler.Persistence;
using Crawler.WebCrawler;

namespace Crawler
{
    class Program
    {
        static uint[] DataSourceSpecificCanonicalIds = new uint[] { 1265954 };

        static void Main(string[] args)
        {
            CrawlerProgress CrawlerProgress = new CrawlerProgress();
            Crawl Crawl = CrawlerProgress.StartCrawl(CrawlerDataSource.MicrosoftAcademicSearch, DataSourceSpecificCanonicalIds.Select(id => id.ToString()).ToArray());

            MASCrawler crawler = new MASCrawler();
            Sources Sources = new Sources();
            CompleteSource cds = null;
            if (Crawl.CrawlState == (short)CrawlerState.Started)
            {
                cds = crawler.GetSourceById(DataSourceSpecificCanonicalIds.First().ToString());
                Sources.AddDetachedSource(cds);
                foreach (uint MasId in DataSourceSpecificCanonicalIds)
                {
                    CrawlerProgress.StoreCanonicalResult(Crawl.CrawlId, MasId.ToString(), cds.Source.SourceId);
                }
                CrawlerProgress.UpdateCrawlerState(Crawl, CrawlerState.CanonicalPaperComplete);
            }
            else
            {
                for (int i = 0; i < DataSourceSpecificCanonicalIds.Length && cds == null; i++)
                {
                    cds = Sources.GetCompleteSourceByCanonicalId(CrawlerDataSource.MicrosoftAcademicSearch, DataSourceSpecificCanonicalIds[i].ToString());
                }
            }

            if(Crawl.CrawlState == (int)CrawlerState.CanonicalPaperComplete)
            {
                foreach (uint MasId in DataSourceSpecificCanonicalIds)
                {
                    string[] PublicationIdsCitingCanonicalPaper = crawler.GetCitationsBySourceId(MasId.ToString());
                    foreach (string DataSourceSpecificId in PublicationIdsCitingCanonicalPaper)
                    {
                        CrawlerProgress.QueueCrawl(Crawl.CrawlId, DataSourceSpecificId, cds.Source.SourceId, CrawlReferenceDirection.Citation);
                    }
                }
                CrawlerProgress.UpdateCrawlerState(Crawl, CrawlerState.EnqueueingCitationsComplete);
            }

            if (Crawl.CrawlState == (int)CrawlerState.EnqueueingCitationsComplete)
            {
                foreach (uint MasId in DataSourceSpecificCanonicalIds)
                {
                    string[] PublicationIdsCitingCanonicalPaper = crawler.GetReferencesBySourceId(MasId.ToString());
                    foreach (string DataSourceSpecificId in PublicationIdsCitingCanonicalPaper)
                    {
                        CrawlerProgress.QueueCrawl(Crawl.CrawlId, DataSourceSpecificId, cds.Source.SourceId, CrawlReferenceDirection.Reference);
                    }
                }
                CrawlerProgress.UpdateCrawlerState(Crawl, CrawlerState.EnqueueingReferencesComplete);
            }
            
        }
    }
}
