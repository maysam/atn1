using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using ATN.Crawler.WokSearch;
using ATN.Crawler.WokSearchLite;
using ATN.Crawler.WOKMWSAuthenticate;
using ATN.Data;
using System.ServiceModel.Security;
using System.Xml;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ATN.Crawler.WebCrawler
{
    /// <summary>
    /// An implementation of the ICrawler interface which is capable of using Microsoft Academic Search as a data source
    /// </summary>
    public class WOKCrawler : ICrawler
    {
        private const int RetryDelayMilliseconds = 5000;
        private const int RetryLimit = 5;
        private const int MaxResultSize = 100;
        private const string WOKDatabaseId = "WOS";
        private const string WOKQueryLanguage = "en";
        
        private RateLimit _limiter;
        WokSearchClient client;
        WokSearchLiteClient lite_client;
        WOKMWSAuthenticateClient auth_client;
        string sid;

        public WOKCrawler()
        {
            _limiter = new RateLimit();
            client = new WokSearchClient();
            auth_client = new WOKMWSAuthenticateClient();
            string authentication_token = auth_client.authenticate();
            lite_client = new WokSearchLiteClient();
            sid = "SID=\"" + authentication_token + "\"";
        }

        public CrawlerDataSource GetDataSource()
        {
            return CrawlerDataSource.WebOfKnowledge;
        }

        public string[] GetCitationsBySourceId(string PaperId)
        {
            Trace.WriteLine(string.Format("Getting citations for publication {0}", PaperId), "Informational");
            List<string> PublicationIdsCitingCanonicalPaper = new List<string>();
            WokSearchLite.timeSpan timespan = new WokSearchLite.timeSpan();
            timespan.begin = "1790-01-01";
            timespan.end = "2015-01-01";
            WokSearchLite.editionDesc[] editions = new WokSearchLite.editionDesc[1];
            editions[0] = new WokSearchLite.editionDesc();
            editions[0].collection = "WOS";
            editions[0].edition = "SCI";

            WokSearchLite.retrieveParameters retrieveParams = new WokSearchLite.retrieveParameters();
            retrieveParams.firstRecord = 1;
            retrieveParams.count = MaxResultSize;
            searchResults results = null;
            int AttemptCount = 0;
            bool InitialRequestSucceeded = false;

            while (!InitialRequestSucceeded && AttemptCount < RetryLimit)
            {
                try
                {
                    using (var scope = new OperationContextScope(lite_client.InnerChannel))
                    {
                        var httpRequestProperty = new HttpRequestMessageProperty();
                        httpRequestProperty.Headers.Add("Cookie", sid);
                        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
                        results = lite_client.citingArticles(WOKDatabaseId, PaperId, editions, timespan, WOKQueryLanguage, retrieveParams);
                        InitialRequestSucceeded = true;
                    }
                }
                catch (MessageSecurityException e)
                {
                    Trace.WriteLine("Exception:", "Error");
                    Trace.WriteLine(e.Message);
                    Trace.WriteLine(e.Source);
                    Trace.WriteLine(e.StackTrace);
                    Trace.WriteLine(e.TargetSite);
                    Trace.WriteLine(e.Data);
                    //"Access Denied", meaning the WOK rate limiter is not happy. Abort immediately.
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
            int ResultCount = results.recordsFound;
            Trace.WriteLine(string.Format("Received first response, {0} papers total.", ResultCount), "Informational");

            AttemptCount = 0;

            while (PublicationIdsCitingCanonicalPaper.Count < ResultCount)
            {

                Trace.WriteLine(string.Format("Retrieving papers {0} through {1}", 1, ResultCount), "Informational");
                foreach (liteRecord p in results.records)
                {
                    //Trace.WriteLine(string.Format("Added paper {0}", p.ID), "Informational");
                    PublicationIdsCitingCanonicalPaper.Add(p.UT);
                }

                while (AttemptCount < RetryLimit && PublicationIdsCitingCanonicalPaper.Count < ResultCount)
                {
                    try
                    {

                        using (var scope = new OperationContextScope(lite_client.InnerChannel))
                        {
                            var httpRequestProperty = new HttpRequestMessageProperty();
                            httpRequestProperty.Headers.Add("Cookie", sid);
                            OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
                            results = lite_client.citingArticles(WOKDatabaseId, PaperId, editions, timespan, WOKQueryLanguage, retrieveParams);
                        }

                        if (results.records.Length == 0)
                        {
                            break;
                        }
                        retrieveParams.firstRecord += MaxResultSize;
                        retrieveParams.count = MaxResultSize;
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
            }

            return PublicationIdsCitingCanonicalPaper.ToArray();
        }

        public string[] GetReferencesBySourceId(string PaperId)
        {
            Trace.WriteLine(string.Format("Getting references for publication {0}", PaperId), "Informational");
            List<string> PublicationIdsCitingCanonicalPaper = new List<string>();

            WokSearch.retrieveParameters retrieveParams = new WokSearch.retrieveParameters();
            retrieveParams.firstRecord = 1;
            retrieveParams.count = MaxResultSize;
            citedReferencesSearchResults results = null;
            int AttemptCount = 0;
            bool InitialRequestSucceeded = false;

            WokSearch.timeSpan timespan = new WokSearch.timeSpan();
            timespan.begin = "1790-01-01";
            timespan.end = "2015-01-01";
            WokSearch.editionDesc[] editions = new WokSearch.editionDesc[1];
            editions[0] = new WokSearch.editionDesc();
            editions[0].collection = "WOS";
            editions[0].edition = "SCI";
            
            while (!InitialRequestSucceeded && AttemptCount < RetryLimit)
            {
                try
                {
                    using (var scope = new OperationContextScope(client.InnerChannel))
                    {
                        var httpRequestProperty = new HttpRequestMessageProperty();
                        httpRequestProperty.Headers.Add("Cookie", sid);
                        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
                        results = client.citedReferences(WOKDatabaseId, PaperId, editions, timespan, WOKQueryLanguage, retrieveParams);                        
                        InitialRequestSucceeded = true;
                    }
                }
                catch (MessageSecurityException e)
                {
                    Trace.WriteLine("Exception:", "Error");
                    Trace.WriteLine(e.Message);
                    Trace.WriteLine(e.Source);
                    Trace.WriteLine(e.StackTrace);
                    Trace.WriteLine(e.TargetSite);
                    Trace.WriteLine(e.Data);
                    //"Access Denied", meaning the WOK rate limiter is not happy. Abort immediately.
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
            int ResultCount = results.recordsFound;
            Trace.WriteLine(string.Format("Received first response, {0} papers total.", ResultCount), "Informational");

            AttemptCount = 0;
            while (PublicationIdsCitingCanonicalPaper.Count < ResultCount)
            {

                Trace.WriteLine(string.Format("Retrieving papers {0} through {1}", 1, ResultCount), "Informational");
                foreach (citedReference p in results.records)
                {
                    //Trace.WriteLine(string.Format("Added paper {0}", p.ID), "Informational");
                    PublicationIdsCitingCanonicalPaper.Add(p.articleID);

                }

                while (AttemptCount < RetryLimit && PublicationIdsCitingCanonicalPaper.Count < ResultCount)
                {
                    try
                    {
                        using (var scope = new OperationContextScope(client.InnerChannel))
                        {
                            var httpRequestProperty = new HttpRequestMessageProperty();
                            httpRequestProperty.Headers.Add("Cookie", sid);
                            OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
                            results = client.citedReferences(WOKDatabaseId, PaperId, editions, timespan, WOKQueryLanguage, retrieveParams);
                        }

                        if (results.records.Length == 0)
                        {
                            break;
                        }

                        retrieveParams.firstRecord += MaxResultSize;
                        retrieveParams.count = MaxResultSize;
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
            }

            PublicationIdsCitingCanonicalPaper.RemoveAll(item => item == null);
            return PublicationIdsCitingCanonicalPaper.ToArray();
        }

        private string getValueByLabel(WokSearchLite.labelValuesPair[] items, string label)
        {
            if(items != null)
                foreach (WokSearchLite.labelValuesPair item in items)
                {
                    if (item.label.Equals(label))
                    {
                        return item.values[0];
                    }
                }
            return null;
        }

        private string getJoinedValuesByLabel(WokSearchLite.labelValuesPair[] items, string label)
        {
            if (items != null)
                foreach (WokSearchLite.labelValuesPair item in items)
                {
                    if (item.label.Equals(label))
                    {
                        StringBuilder builder = new StringBuilder();
                        foreach (string value in item.values)
                        {
                            builder.Append(value);
                            builder.Append('.');
                        }
                        return builder.ToString();

                    }
                }
            return null;
        }

        public CompleteSource GetSourceById(string PaperId)
        {
            WokSearchLite.retrieveParameters _retrieveParameters = new WokSearchLite.retrieveParameters();
            _retrieveParameters.count = 1;
            _retrieveParameters.firstRecord = 1;
            
            string[] uids = new string[1];
            uids[0] = PaperId;
            searchResults results = null;
            for (int i = 1; i <= RetryLimit; i++)
            {
                try
                {

                    using (var scope = new OperationContextScope(lite_client.InnerChannel))
                    {
                        var httpRequestProperty = new HttpRequestMessageProperty();
                        httpRequestProperty.Headers.Add("Cookie", sid);
                        OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;
                        results = lite_client.retrieveById(WOKDatabaseId, uids, WOKQueryLanguage, _retrieveParameters);
                        break;
                    }
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
            liteRecord RetrievedPublication = results.records[0];
            Source CanonicalPaper = new Source();
            //CanonicalPaper.Abstract = RetrievedPublication.Abstract;
            if (RetrievedPublication.title != null)
                if (RetrievedPublication.title[0].values != null)
                    CanonicalPaper.ArticleTitle = RetrievedPublication.title[0].values[0];
            CanonicalPaper.DataSourceId = (int)CrawlerDataSource.WebOfKnowledge;
            CanonicalPaper.DataSourceSpecificId = PaperId;
            int year;
            Int32.TryParse(getValueByLabel(RetrievedPublication.source, "Published.BiblioYear"), out year);
            CanonicalPaper.Year = year;
            CanonicalPaper.SerializedDataSourceResponse = XmlHelper.XmlSerialize(results);
            CanonicalPaper.DOI = getValueByLabel(RetrievedPublication.other, "Identifier.Xref_Doi");
/*
            if(RetrievedPublication.FullVersionURL.Length > 0)
            {
                CanonicalPaper.ExternalURL = RetrievedPublication.FullVersionURL.FirstOrDefault();
            }
            */
            List<ATN.Data.Author> Authors = new List<ATN.Data.Author>(RetrievedPublication.authors.Length);
            int author_i = 0;
            if(RetrievedPublication.authors != null)
            foreach (string Author in RetrievedPublication.authors[0].values)
            {
                author_i++;
                var AuthorToAdd = new ATN.Data.Author();
                char[] delimiterChars = { ',' };
                string[] parts = Author.Split(delimiterChars);
                AuthorToAdd.FirstName = parts[1].Trim();
                AuthorToAdd.LastName = parts[0];
                AuthorToAdd.FullName = Author;
                AuthorToAdd.DataSourceSpecificId = PaperId + "." + author_i;
                AuthorToAdd.DataSourceId = (int)CrawlerDataSource.WebOfKnowledge;
                Authors.Add(AuthorToAdd);
            }

            string sourceTitle = getValueByLabel(RetrievedPublication.source, "SourceTitle");
            if (sourceTitle != null)
            {
                ATN.Data.Journal Journal = new ATN.Data.Journal();
                Journal.JournalName = sourceTitle;
                cs.Journal = Journal;
            }
            cs.IsDetached = true;
            cs.Authors = Authors.ToArray();
            cs.Source = CanonicalPaper;

            return cs;
        }
    }
}
