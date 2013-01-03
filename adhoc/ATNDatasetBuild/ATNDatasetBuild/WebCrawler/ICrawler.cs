using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crawler.Persistence;

namespace Crawler.WebCrawler
{
    public interface ICrawler
    {
        string[] GetCitationsBySourceId(string CanonicalId);
        string[] GetReferencesBySourceId(string PaperId);
        CompleteSource GetSourceById(string PaperId); 
    }
}
