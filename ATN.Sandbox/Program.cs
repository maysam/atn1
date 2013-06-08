using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using ATN.Analysis;
using ATN.Export;
using ATN.Data;
using System.IO;
using System.Threading;
using ATN.Import;
using ATN.Crawler.WebCrawler;
using ATN.Crawler.MAS;

namespace ATN.Analysis
{
    class Program
    {
        public static void Main(string[] args)
        {
            Sources s = new Sources();
            Source[] AllSources = s.GetSources();
            foreach (Source CurrentSource in AllSources)
            {
                Response r = XmlHelper.XmlDeserialize<Response>(CurrentSource.SerializedDataSourceResponse);
                if (r.Publication != null)
                {
                    string FullVersionURL = r.Publication.Result[0].FullVersionURL.FirstOrDefault();
                    if (!string.IsNullOrEmpty(FullVersionURL))
                    {
                        s.UpdateExternalURL(CurrentSource, FullVersionURL);
                    }
                }
            }
        }
    }
}