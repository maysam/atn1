using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crawler.Persistence;

namespace Crawler.WebCrawler
{
    public interface ICrawler
    {
        /// <summary>
        /// Get the data-source specific identifiers listed as citing the provided canonical paper
        /// </summary>
        /// <param name="CanonicalId">The data-source specific identifier of the canonical paper with which to retrieve citations for</param>
        /// <returns>A list of citations for the given canonical paper</returns>
        string[] GetCitationsBySourceId(string CanonicalId);
        /// <summary>
        /// Get the list of data-source specific identifiers that the provided paper cites
        /// </summary>
        /// <param name="PaperId">The data-source specific identifier of the paper with which to retrieve references for</param>
        /// <returns>A list of references for the given paper</returns>
        string[] GetReferencesBySourceId(string PaperId);
        /// <summary>
        /// Retrieves a detached copy of a source, authors, editors, and journal from the data source
        /// </summary>
        /// <param name="PaperId">The data-source specific identifier of the source to retrieve</param>
        /// <returns>A detached copy of a source, authors, editors, and journal from the data source</returns>
        CompleteSource GetSourceById(string PaperId); 
    }
}
