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
            ICrawler crawler = new MASCrawler();
            CrawlRunner co = new CrawlRunner();
            co.RunCrawls(
                new CrawlSpecifier[] {
                    new CrawlSpecifier(){
                        DataSource = CrawlerDataSource.MicrosoftAcademicSearch,
                        DataSourceSpecificIdentifiers = new string[] { "1331038" }
                    }
                }
            );
            co.RefreshExistingCrawls();
        }
    }
}
