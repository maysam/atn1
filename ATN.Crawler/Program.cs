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
            /*co.RunCrawls(
                    new CrawlSpecifier(CrawlerDataSource.MicrosoftAcademicSearch, "2085496")
                    //,new CrawlSpecifier(...)
            );*/
            co.RefreshExistingCrawls();
        }
    }
}
