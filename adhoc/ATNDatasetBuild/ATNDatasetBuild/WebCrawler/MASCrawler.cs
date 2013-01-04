using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crawler.MAS;
using System.Diagnostics;
using Crawler.Persistence;
using System.Threading;

namespace Crawler.WebCrawler
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
        private string[] GetReferences(string CanonicalId, ReferenceRelationship Relationship)
        {
            List<uint> PublicationIdsCitingCanonicalPaper = new List<uint>();

            Request request = new Request();
            request.AppID = MASAppId;
            request.OrderBy = OrderType.Year;

            //Get publication data
            request.ResultObjects = ObjectType.Publication;
            request.ReferenceType = Relationship;

            request.PublicationID = UInt32.Parse(CanonicalId);
            request.StartIdx = 1;
            request.EndIdx = request.StartIdx + MaxResultSize - 1;

            //Get response
            _limiter.AddRequest();
            Response response = _client.Search(request);
            int RetrievedItems = 0;
            uint ResultCount = response.Publication.TotalItem;
            Trace.WriteLine(string.Format("Received first response, {0} papers total.", ResultCount), "Informational");

            while (RetrievedItems < ResultCount)
            {
                Trace.WriteLine(string.Format("Retrieving papers {0} through {1}", request.StartIdx, request.EndIdx), "Informational");
                RetrievedItems += response.Publication.Result.Length;
                foreach (var p in response.Publication.Result)
                {
                    Trace.WriteLine(string.Format("Added paper {0}", p.ID), "Informational");
                    PublicationIdsCitingCanonicalPaper.Add(p.ID);
                }
                request.StartIdx += MaxResultSize;
                request.EndIdx = request.StartIdx + MaxResultSize - 1;

                _limiter.AddRequest();
                response = _client.Search(request);
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

            _limiter.AddRequest();
            Publication RetrievedPublication = null;
            Response response = null;
            for (int i = 1; i <= RetryLimit; i++)
            {
                try
                {
                    response = _client.Search(request);
                    RetrievedPublication = response.Publication.Result.SingleOrDefault();
                    break;
                }
                catch (Exception e)
                {
                    if (i == RetryLimit)
                    {
                        throw e;
                    }
                    Thread.Sleep(RetryDelayMilliseconds);
                }
            }

            Trace.WriteLine(string.Format("Retrieved source {0}", PaperId), "Informational");

            CompleteSource cs = new CompleteSource();

            Source CanonicalPaper = new Source();
            CanonicalPaper.Abstract = RetrievedPublication.Abstract;
            CanonicalPaper.ArticleTitle = RetrievedPublication.Title;
            CanonicalPaper.DataSourceId = (int)CrawlerDataSource.MicrosoftAcademicSearch;
            CanonicalPaper.MasID = Int32.Parse(PaperId);
            CanonicalPaper.Year = RetrievedPublication.Year;
            CanonicalPaper.SerializedDataSourceResponse = XmlHelper.XmlSerialize(response);
            CanonicalPaper.DOI = RetrievedPublication.DOI;

            List<Persistence.Author> Authors = new List<Persistence.Author>(RetrievedPublication.Author.Length);
            foreach (var Author in RetrievedPublication.Author)
            {
                var AuthorToAdd = new Persistence.Author();
                AuthorToAdd.FirstName = Author.FirstName;
                AuthorToAdd.LastName = Author.LastName;
                AuthorToAdd.FullName = Author.FirstName + " " + Author.MiddleName + " " + Author.LastName;
                AuthorToAdd.DataSourceSpecificId = Author.ID.ToString();
                AuthorToAdd.DataSourceId = (int)CrawlerDataSource.MicrosoftAcademicSearch;
                Authors.Add(AuthorToAdd);
            }

            Persistence.Journal Journal = new Persistence.Journal();
            if(RetrievedPublication.Journal != null)
            {
                Journal.JournalName = RetrievedPublication.Journal.FullName;
                cs.Journal = Journal;
            }

            cs.IsDetached = true;
            cs.Authors = Authors.ToArray();
            cs.Source = CanonicalPaper;
            
            return cs;
        }

        string[] ICrawler.GetCitationsBySourceId(string CanonicalId)
        {
            throw new NotImplementedException();
        }

        string[] ICrawler.GetReferencesBySourceId(string PaperId)
        {
            throw new NotImplementedException();
        }

        CompleteSource ICrawler.GetSourceById(string PaperId)
        {
            throw new NotImplementedException();
        }
    }
}
