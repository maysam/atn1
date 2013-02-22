using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using ATN.Crawler.MAS;
using ATN.Data;
using System.ServiceModel.Security;

namespace ATN.Crawler.WebCrawler
{
    /// <summary>
    /// An implementation of the ICrawler interface which is capable of using Microsoft Academic Search as a data source
    /// </summary>
    public class MASCrawler : ICrawler
    {
        private const int RetryDelayMilliseconds = 5000;
        private const int RetryLimit = 5;
        private const int MaxResultSize = 100;
        private const string MASAppId = "d34a7152-9c60-4c50-80b3-24435cb20a27";
        private RateLimit _limiter;
        APIServiceClient _client;

        public MASCrawler()
        {
            _limiter = new RateLimit();
            _client = new APIServiceClient();
        }

        public CrawlerDataSource GetDataSource()
        {
            return CrawlerDataSource.MicrosoftAcademicSearch;
        }

        /// <summary>
        /// Retrieves a list of references or citations for the given PublicationId
        /// </summary>
        /// <param name="PublicationId">The MAS-specific unique identifier corresponding to the Publication to retrieve references for</param>
        /// <param name="Relationship">The type of references to retrieve; citations or references</param>
        /// <returns>A list of MAS-specific unique identifiers referencing or citing the Publication provided</returns>
        private string[] GetReferences(string PublicationId, ReferenceRelationship Relationship)
        {
            List<uint> PublicationIdsCitingCanonicalPaper = new List<uint>();

            Request request = new Request();
            request.AppID = MASAppId;
            request.OrderBy = OrderType.Year;

            //Get publication data
            request.ResultObjects = ObjectType.Publication;
            request.ReferenceType = Relationship;

            request.PublicationID = UInt32.Parse(PublicationId);
            request.StartIdx = 1;
            request.EndIdx = request.StartIdx + MaxResultSize - 1;

            //Get response
            int AttemptCount = 0;
            bool InitialRequestSucceeded = false;
            uint ResultCount = 0;
            Response response = null;
            while(!InitialRequestSucceeded && AttemptCount < RetryLimit)
            {
                try
                {
                    response = _client.Search(request, _limiter);
                    ResultCount = response.Publication.TotalItem;
                    InitialRequestSucceeded = true;
                }
                catch (MessageSecurityException e)
                {
                    Trace.WriteLine("Exception:", "Error");
                    Trace.WriteLine(e.Message);
                    Trace.WriteLine(e.Source);
                    Trace.WriteLine(e.StackTrace);
                    Trace.WriteLine(e.TargetSite);
                    Trace.WriteLine(e.Data);
                    //"Access Denied", meaning the MAS rate limiter is not happy. Abort immediately.
                    throw e;
                }
                catch (Exception e)
                {
                    AttemptCount++;
                    Trace.WriteLine("Exception:", "Error");
                    Trace.WriteLine(e.Message);
                    Trace.WriteLine(e.Source);
                    Trace.WriteLine(e.StackTrace);
                    Trace.WriteLine(e.TargetSite);
                    Trace.WriteLine(e.Data);
                    if (AttemptCount == RetryLimit)
                    {
                        throw e;
                    }
                    Thread.Sleep(AttemptCount * RetryDelayMilliseconds);
                }
            }
            Trace.WriteLine(string.Format("Received first response, {0} papers total.", ResultCount), "Informational");

            AttemptCount = 0;
            while (PublicationIdsCitingCanonicalPaper.Count < ResultCount)
            {
                
                Trace.WriteLine(string.Format("Retrieving papers {0} through {1}", request.StartIdx, request.EndIdx), "Informational");
                foreach (var p in response.Publication.Result)
                {
                    //Trace.WriteLine(string.Format("Added paper {0}", p.ID), "Informational");
                    PublicationIdsCitingCanonicalPaper.Add(p.ID);
                }

                while (AttemptCount < RetryLimit && PublicationIdsCitingCanonicalPaper.Count < ResultCount)
                {
                    try
                    {

                        response = _client.Search(request, _limiter);

                        if (response.Publication.Result.Length == 0)
                        {
                            break;
                        }

                        request.StartIdx += MaxResultSize;
                        request.EndIdx = request.StartIdx + MaxResultSize - 1;
                        break;
                    }
                    catch (Exception e)
                    {
                        AttemptCount++;
                        Trace.WriteLine("Exception:", "Error");
                        Trace.WriteLine(e.Message);
                        Trace.WriteLine(e.Source);
                        Trace.WriteLine(e.StackTrace);
                        Trace.Write(e.TargetSite);
                        Trace.Write(e.Data);
                        if (AttemptCount == RetryLimit)
                        {
                            throw e;
                        }
                        Thread.Sleep(AttemptCount * RetryDelayMilliseconds);
                    }
                }

                if(response.Publication != null && response.Publication.Result != null && response.Publication.Result.Length == 0)
                {
                    //Not retrieving any results; there's something wrong with the request
                    //Break so as to not continue using API requests to ask for data which
                    //is not there
                    break;
                }
            }

            return PublicationIdsCitingCanonicalPaper.Select(val => val.ToString()).ToArray();
        }
        public string[] GetCitationsBySourceId(string PaperId)
        {
            Trace.WriteLine(string.Format("Getting citations for publication {0}", PaperId), "Informational");
            return GetReferences(PaperId, ReferenceRelationship.Citation);
        }

        public string[] GetReferencesBySourceId(string PaperId)
        {
            Trace.WriteLine(string.Format("Getting references for publication {0}", PaperId), "Informational");
            return GetReferences(PaperId, ReferenceRelationship.Reference);
        }

        public CompleteSource GetSourceById(string PaperId)
        {
            Request request = new Request();
            request.AppID = MASAppId;
            request.OrderBy = OrderType.Year;
            request.StartIdx = 1;
            request.EndIdx = 1;

            request.PublicationID = UInt32.Parse(PaperId);

            request.ResultObjects = ObjectType.Publication;
            request.PublicationContent = new PublicationContentType[] { PublicationContentType.AllInfo };

            Publication RetrievedPublication = null;
            Response response = null;
            for (int i = 1; i <= RetryLimit; i++)
            {
                try
                {
                    response = _client.Search(request, _limiter);
                    RetrievedPublication = response.Publication.Result.SingleOrDefault();
                    break;
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Exception:", "Error");
                    Trace.WriteLine(e.Message);
                    Trace.WriteLine(e.Source);
                    Trace.WriteLine(e.StackTrace);
                    Trace.WriteLine(e.TargetSite);
                    Trace.WriteLine(e.Data);
                    if (i == RetryLimit)
                    {
                        throw e;
                    }
                    Thread.Sleep(i * RetryDelayMilliseconds);
                }
            }

            Trace.WriteLine(string.Format("Retrieved source {0}", PaperId), "Informational");

            CompleteSource cs = new CompleteSource();

            Source CanonicalPaper = new Source();
            CanonicalPaper.Abstract = RetrievedPublication.Abstract;
            CanonicalPaper.ArticleTitle = RetrievedPublication.Title;
            CanonicalPaper.DataSourceId = (int)CrawlerDataSource.MicrosoftAcademicSearch;
            CanonicalPaper.DataSourceSpecificId = PaperId;
            CanonicalPaper.Year = RetrievedPublication.Year;
            CanonicalPaper.SerializedDataSourceResponse = XmlHelper.XmlSerialize(response);
            CanonicalPaper.DOI = RetrievedPublication.DOI;

            List<ATN.Data.Author> Authors = new List<ATN.Data.Author>(RetrievedPublication.Author.Length);
            foreach (var Author in RetrievedPublication.Author)
            {
                var AuthorToAdd = new ATN.Data.Author();
                AuthorToAdd.FirstName = Author.FirstName;
                AuthorToAdd.LastName = Author.LastName;
                AuthorToAdd.FullName = Author.FirstName + " " + Author.MiddleName + " " + Author.LastName;
                AuthorToAdd.DataSourceSpecificId = Author.ID.ToString();
                AuthorToAdd.DataSourceId = (int)CrawlerDataSource.MicrosoftAcademicSearch;
                Authors.Add(AuthorToAdd);
            }

            ATN.Data.Journal Journal = new ATN.Data.Journal();
            if (RetrievedPublication.Journal != null)
            {
                Journal.JournalName = RetrievedPublication.Journal.FullName;
                cs.Journal = Journal;
            }

            cs.IsDetached = true;
            cs.Authors = Authors.ToArray();
            cs.Source = CanonicalPaper;

            return cs;
        }
    }
    
    public static class MASReferenceExtensions
    {
        private const int WaitDelayMinutes = 10;
        private const int MillisecondsPerMinute = 60000;
        public static Response Search(this APIServiceClient client, Request request, RateLimit limiter)
        {
            limiter.AddRequest();
            Response response = client.Search(request);
            HandleResultCode(response.ResultCode);
            return response;
        }
        private static void HandleResultCode(uint ResultCode)
        {
            switch (ResultCode)
            {
                case 1:
                    //AppID not authorized. MAS throws this when it is under heavly load, so cease further requests
                    Thread.Sleep(MillisecondsPerMinute * 2 * WaitDelayMinutes);
                    throw new Exception("MAS request failed; please try again.");
                case 2:
                    //Search parameter is incorrect; this means there is a bug in the crawler code
                    Environment.Exit(1);
                    break;
                case 3:
                    //MAS is temporarily unavailable; wait 10 minutes to continue
                    Thread.Sleep(MillisecondsPerMinute * WaitDelayMinutes);
                    throw new Exception("MAS request failed; please try again.");
                case 4:
                    //Search method unsupported; this means there is a bug in the crawler code
                    Thread.Sleep(MillisecondsPerMinute * WaitDelayMinutes);
                    Environment.Exit(1);
                    throw new Exception("MAS request failed; please try again.");
            }
        }
    }
}
