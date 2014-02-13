﻿using System;
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
        private authenticateResponse authentication_identifier;

        public WOKCrawler()
        {
            _limiter = new RateLimit();
            client = new WokSearchClient();
            auth_client = new WOKMWSAuthenticateClient();
            //            authentication_identifier = auth_client.authenticate();
            //            _client.ClientCredentials.IssuedToken = authentication_identifier.Body.@return;
            string authentication_token = auth_client.authenticate();
            // somehow authentication_token needs to be added to client header 
            // $search_client->__setCookie('SID',$auth_response->return);

            client.Open();

            lite_client = new WokSearchLiteClient();
            lite_client.Open();
        }

        public CrawlerDataSource GetDataSource()
        {
            return CrawlerDataSource.WebOfKnowledge;
        }

        public string[] GetCitationsBySourceId(string PaperId)
        {
            Trace.WriteLine(string.Format("Getting citations for publication {0}", PaperId), "Informational");
            List<string> PublicationIdsCitingCanonicalPaper = new List<string>();

            WokSearchLite.retrieveParameters retrieveParams = new WokSearchLite.retrieveParameters();
            retrieveParams.firstRecord = 1;
            retrieveParams.count = MaxResultSize;
            WokSearchLite.queryField sorting = new WokSearchLite.queryField();
            sorting.name = "YEAR";
            sorting.sort = "A";
            retrieveParams.fields = new WokSearchLite.queryField[1];
            retrieveParams.fields.SetValue(sorting, 0);
//            retrieveParams.fields[0].name = "YEAR";
//            retrieveParams.fields[0].sort = "A";
            searchResults results = null;
            int AttemptCount = 0;
            bool InitialRequestSucceeded = false;

            while (!InitialRequestSucceeded && AttemptCount < RetryLimit)
            {
                try
                {
                    results = lite_client.citingArticles(WOKDatabaseId, PaperId, null, null, WOKQueryLanguage, retrieveParams);
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

                        results = lite_client.citingArticles(WOKDatabaseId, PaperId, null, null, WOKQueryLanguage, retrieveParams);

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
            retrieveParams.fields[0].name = "YEAR";
            retrieveParams.fields[0].sort = "A";
            citedReferencesSearchResults results = null;
            int AttemptCount = 0;
            bool InitialRequestSucceeded = false;

            while (!InitialRequestSucceeded && AttemptCount < RetryLimit)
            {
                try
                {
                    results = client.citedReferences(WOKDatabaseId, PaperId, null, null, WOKQueryLanguage, retrieveParams);

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
                        results = client.citedReferences(WOKDatabaseId, PaperId, null, null, WOKQueryLanguage, retrieveParams);

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

        private string getValueByLabel(WokSearchLite.labelValuesPair[] items, string label)
        {
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
            string databaseId = "WOS:A1970Y327100002";
            databaseId = WOKDatabaseId;
            WokSearchLite.retrieveParameters _retrieveParameters = new WokSearchLite.retrieveParameters();
            _retrieveParameters.count = 1;
            _retrieveParameters.firstRecord = 1;
            _retrieveParameters.fields[0].name = "AU";
            _retrieveParameters.fields[0].sort = "A";
            
            string queryLanguage = "en";
            string[] uids = new string[1];
            uids[0] = PaperId;
            searchResults results = null;
            for (int i = 1; i <= RetryLimit; i++)
            {
                try
                {
                    results = lite_client.retrieveById(databaseId, uids, queryLanguage, _retrieveParameters);
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
            liteRecord RetrievedPublication = results.records[0];
            Source CanonicalPaper = new Source();
            //CanonicalPaper.Abstract = RetrievedPublication.Abstract;
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
            foreach (string Author in RetrievedPublication.authors[0].values)
            {
                var AuthorToAdd = new ATN.Data.Author();
                //AuthorToAdd.FirstName = Author.FirstName;
                //AuthorToAdd.LastName = Author.LastName;
                AuthorToAdd.FullName = Author;
                //AuthorToAdd.DataSourceSpecificId = Author.ID.ToString();
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
            /*
            List<Subject> Subjects = new List<Subject>();
            foreach (Keyword k in RetrievedPublication.Keyword)
            {
                Subject s = new Subject();
                s.DataSourceSpecificId = k.ID.ToString();
                s.DataSourceId = (int)GetDataSource();
                s.SubjectText = k.Name;
                Subjects.Add(s);
            }
            cs.Subjects = Subjects.ToArray();
             */
            cs.IsDetached = true;
            cs.Authors = Authors.ToArray();
            cs.Source = CanonicalPaper;

            return cs;
        }
    }
}
