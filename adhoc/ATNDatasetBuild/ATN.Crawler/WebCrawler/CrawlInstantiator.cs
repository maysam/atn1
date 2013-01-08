using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATN.Data;

namespace ATN.Crawler.WebCrawler
{
    public static class CrawlInstantiator
    {
        public static ICrawler InstantiateCrawler(CrawlerDataSource DataSource)
        {
            return ConfigurationReader.GetConfigurationValue<string>("DataSourceObjectTranslations")
                .Split(
                    new string[] { "," },
                    StringSplitOptions.RemoveEmptyEntries
                ).Select(
                    s => s.Trim().Replace(" ", string.Empty)
                ).Select(
                    s => s.Split(new string[] { "=>" },
                    StringSplitOptions.RemoveEmptyEntries)
                ).ToDictionary(
                    s => (CrawlerDataSource)Int32.Parse(s[0]),
                    (s) => (ICrawler)Activator.CreateInstance(
                        Type.GetType("ATN.Crawler.WebCrawler." + s[1])
                        )
                )[DataSource];
        }
    }
}
