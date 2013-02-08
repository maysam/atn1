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
            Theories t = new Theories();
            /*t.AddTheory("Technology Acceptance Model",
                new string[][] {
                    new string[] {"1265954", "1279051"},
                    new string[] {"1253523", "38747179"}
                }
            );*/
            CrawlRunner co = new CrawlRunner();
            /*co.StartNewCrawl(
                    new NewCrawlSpecifier(
                        "Technology Acceptance Model",
                        CrawlerDataSource.MicrosoftAcademicSearch,
                        new string[][] {
                            new string[] {"1265954", "1279051"},
                            new string[] {"1253523", "38747179"}
                        }
                    )
            );*/
            co.RefreshExistingCrawls();
        }
    }
}
