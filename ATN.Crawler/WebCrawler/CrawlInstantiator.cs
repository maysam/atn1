using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATN.Data;

namespace ATN.Crawler.WebCrawler
{
    public static class CrawlInstantiator
    {
        private static Dictionary<CrawlerDataSource, ICrawler> DataSourceCrawlerTranslations;
        
        /// <summary>
        /// Generates a translation between any CrawlerDataSource value and its target implementation
        /// </summary>
        private static void PopulateTranslations()
        {
            DataSourceCrawlerTranslations = 
                ConfigurationReader.GetConfigurationValue<string>("DataSourceObjectTranslations")
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
                    s => (ICrawler)Activator.CreateInstance(
                        Type.GetType("ATN.Crawler.WebCrawler." + s[1])
                    )
                );
        }
        /// <summary>
        /// Retrieves an instance of the ICrawler implementation corresponding to the given data source
        /// </summary>
        /// <param name="DataSource">The data source to retrieve the ICrawler implementation for</param>
        /// <returns>An instance of the ICrawler implementation corresponding to the given data source</returns>
        public static ICrawler InstantiateCrawler(CrawlerDataSource DataSource)
        {
            if (DataSourceCrawlerTranslations != null)
            {
                return DataSourceCrawlerTranslations[DataSource];
            }
            else
            {
                PopulateTranslations();
                return DataSourceCrawlerTranslations[DataSource];
            }
        }
        /// <summary>
        /// Retrieves a Dictionary keyed by CrawlerDataSource that allows translating a given data source to its corresponding ICrawler implementation
        /// </summary>
        /// <returns>A Dictionary keyed by CrawlerDataSource that allows translating a given data source to its corresponding ICrawler implementation</returns>
        public static Dictionary<CrawlerDataSource, ICrawler> RetrieveCrawlerTranslations()
        {
            if (DataSourceCrawlerTranslations != null)
            {
                return DataSourceCrawlerTranslations;
            }
            else
            {
                PopulateTranslations();
                return DataSourceCrawlerTranslations;
            }
        }
    }
}
