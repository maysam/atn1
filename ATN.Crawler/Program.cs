using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATN.Crawler.MAS;
using ATN.Crawler.WebCrawler;
using System.Diagnostics;
using ATN.Data;

namespace ATN.Crawler
{
    class Program
    {
        static void Main(string[] args)
        {
            CrawlRunner co = new CrawlRunner();
            /*co.StartNewCrawl(
                    new NewCrawlSpecifier(
                        "Transtheoretical Model/Stages of Change",
                        new CanonicalDataSource(CrawlerDataSource.MicrosoftAcademicSearch, "37035751"),
                        new CanonicalDataSource(CrawlerDataSource.MicrosoftAcademicSearch, "36978289", "36978290"),
                        new CanonicalDataSource(CrawlerDataSource.MicrosoftAcademicSearch, "3228451", "45552775")
                    )
            );*/
            co.RefreshExistingCrawls();
        }
    }
}
