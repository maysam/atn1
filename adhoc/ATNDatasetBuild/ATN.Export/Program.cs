using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATN.Data;

namespace ATN.Export
{
    class Program
    {
        static void Main(string[] args)
        {
            CrawlerProgress Progress = new CrawlerProgress();
            string[] MasIds = Progress.GetCanonicalIdsForCrawl(CrawlerDataSource.MicrosoftAcademicSearch, 1);

            SourceRetrievalService Retrival = new SourceRetrievalService();
            Source CanonicalSource = Retrival.GetSourceByDataSourceSpecificId(CrawlerDataSource.MicrosoftAcademicSearch, MasIds[0]);
            Source[] CitingSources = CanonicalSource.Sources.ToArray();

            int x = 0;
        }
    }
}
