using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crawler.MAS;
using Crawler.Persistence;
using Crawler.WebCrawler;
using System.Diagnostics;

namespace Crawler
{
    class Program
    {
        static uint[] DataSourceSpecificCanonicalIds = new uint[] { 37035751 };

        static void Main(string[] args)
        {
            CrawlerProgress CrawlerProgress = new CrawlerProgress();
            Crawl Crawl = CrawlerProgress.StartCrawl(CrawlerDataSource.MicrosoftAcademicSearch, DataSourceSpecificCanonicalIds.Select(id => id.ToString()).ToArray());

            MASCrawler crawler = new MASCrawler();
            Sources Sources = new Sources();
            SourceRetrievalService SourceRetrieval = new SourceRetrievalService();
            CompleteSource cds = null;
            if (Crawl.CrawlState == (short)CrawlerState.Started)
            {
                Trace.WriteLine("Crawl data not present, initiating crawl", "Informational");
                cds = crawler.GetSourceById(DataSourceSpecificCanonicalIds.First().ToString());
                Source CanonicalSource = SourceRetrieval.GetSourceByDataSourceSpecificId(CrawlerDataSource.MicrosoftAcademicSearch, DataSourceSpecificCanonicalIds.First().ToString());
                if (CanonicalSource == null)
                {
                   CanonicalSource = Sources.AddDetachedSource(cds);
                }
                foreach (uint MasId in DataSourceSpecificCanonicalIds)
                {
                    CrawlerProgress.StoreCanonicalResult(Crawl.CrawlId, MasId.ToString(), cds.Source.SourceId);
                }
                CrawlerProgress.UpdateCrawlerState(Crawl, CrawlerState.CanonicalPaperComplete);
                Trace.WriteLine("Canonical paper retrieved, crawl state committed", "Informational");
            }
            else
            {
                Trace.WriteLine("Crawl already started, retrieving crawl state from database model", "Informational");
                for (int i = 0; i < DataSourceSpecificCanonicalIds.Length && cds == null; i++)
                {
                    cds = Sources.GetCompleteSourceByCanonicalId(CrawlerDataSource.MicrosoftAcademicSearch, DataSourceSpecificCanonicalIds[i].ToString());
                }
            }

            if(Crawl.CrawlState == (int)CrawlerState.CanonicalPaperComplete)
            {
                Trace.WriteLine("Queueing citations", "Informational");
                foreach (uint MasId in DataSourceSpecificCanonicalIds)
                {
                    string[] PublicationIdsCitingCanonicalPaper = crawler.GetCitationsBySourceId(MasId.ToString());
                    CrawlerProgress.QueueCrawl(Crawl.CrawlId, PublicationIdsCitingCanonicalPaper, cds.Source.SourceId, CrawlReferenceDirection.Citation);
                    Trace.WriteLine(string.Format("Queued {0} citations", PublicationIdsCitingCanonicalPaper.Length));
                }
                CrawlerProgress.UpdateCrawlerState(Crawl, CrawlerState.EnqueueingCitationsComplete);
                Trace.WriteLine("Citation queueing complete", "Informational");
            }

            if (Crawl.CrawlState == (int)CrawlerState.EnqueueingCitationsComplete)
            {
                Trace.WriteLine("Dequeueing and retreiving citations", "Informational");
                CrawlQueue[] CrawlsToComplete = CrawlerProgress.GetPendingCrawlsForCrawlId(Crawl.CrawlId);
                Trace.WriteLine(string.Format("Retreiving {0} citations", CrawlsToComplete.Length), "Informational");
                for(int i = 0; i < CrawlsToComplete.Length; i++)
                {
                    if(CrawlsToComplete[i].CrawlReferenceDirection == (int)CrawlReferenceDirection.Citation)
                    {
                        Source SourceToComplete = SourceRetrieval.GetSourceByDataSourceSpecificId(CrawlerDataSource.MicrosoftAcademicSearch, CrawlsToComplete[i].DataSourceSpecificId);
                        if (SourceToComplete == null)
                        {
                            CompleteSource SourceToAdd = crawler.GetSourceById(CrawlsToComplete[i].DataSourceSpecificId);
                            Sources.AddDetachedSource(SourceToAdd);
                            SourceToComplete = SourceToAdd.Source;
                            Trace.WriteLine("Source does not exist in database, adding.", "Informational");
                        }
                        try
                        {
                            Sources.AddReference(SourceToComplete.SourceId, CrawlsToComplete[i].ReferencesSourceId.Value);
                        }
                        catch
                        {
                            Sources = new Sources();
                            Trace.WriteLine(string.Format("Source ID {0} already cites Source ID {1}", SourceToComplete.SourceId, CrawlsToComplete[i].ReferencesSourceId.Value), "Informational");
                        }
                        CrawlerProgress.CompleteQueueItem(CrawlsToComplete[i], SourceToComplete.SourceId);
                        Trace.WriteLine(string.Format("Dequeued and retrieved {0}/{1}, Source ID: {2}, Data-Source Specific ID: {3}", i + 1, CrawlsToComplete.Length, SourceToComplete.SourceId, CrawlsToComplete[i].DataSourceSpecificId), "Informational");
                    }
                }
                CrawlerProgress.UpdateCrawlerState(Crawl, CrawlerState.RetrievingCitationsComplete);
                Trace.WriteLine("Dequeueing citations complete", "Informational");
            }

            if (Crawl.CrawlState == (int)CrawlerState.RetrievingCitationsComplete)
            {
                Trace.WriteLine("Queueing references", "Informational");
                var CrawlResults = Crawl.CrawlResults.Where(cr => cr.ReferencesRetrieved == false).ToArray();
                for (int i = 0; i < CrawlResults.Length; i++)
                {
                    Trace.WriteLine(string.Format("Queueing references for paper {0}/{1}", i + 1, CrawlResults.Length));
                    string[] PublicationIdsPaperReferences = crawler.GetReferencesBySourceId(CrawlResults[i].DataSourceSpecificId);
                    CrawlerProgress.QueueCrawl(Crawl.CrawlId, PublicationIdsPaperReferences, CrawlResults[i].SourceId, CrawlReferenceDirection.Reference);
                    CrawlResults[i].ReferencesRetrieved = true;
                    CrawlerProgress.Context.SaveChanges();
                    Trace.WriteLine(string.Format("Queued {0} references for paper {1}/{2}", PublicationIdsPaperReferences.Length, i + 1, CrawlResults.Length));
                }
                CrawlerProgress.UpdateCrawlerState(Crawl, CrawlerState.EnqueueingReferencesComplete);
                Trace.WriteLine("Reference queueing complete", "Informational");
            }

            if (Crawl.CrawlState == (int)CrawlerState.EnqueueingReferencesComplete)
            {
                Trace.WriteLine("Dequeueing and retreiving references", "Informational");
                CrawlQueue[] CrawlsToComplete = CrawlerProgress.GetPendingCrawlsForCrawlId(Crawl.CrawlId);
                Trace.WriteLine(string.Format("Retreiving {0} queued references", CrawlsToComplete.Length), "Informational");
                for (int i = 0; i < CrawlsToComplete.Length; i++)
                {
                    if (CrawlsToComplete[i].CrawlReferenceDirection == (int)CrawlReferenceDirection.Reference)
                    {
                        Source SourceToComplete = SourceRetrieval.GetSourceByDataSourceSpecificId(CrawlerDataSource.MicrosoftAcademicSearch, CrawlsToComplete[i].DataSourceSpecificId);
                        if (SourceToComplete == null)
                        {
                            CompleteSource SourceToAdd = crawler.GetSourceById(CrawlsToComplete[i].DataSourceSpecificId);
                            Sources.AddDetachedSource(SourceToAdd);
                            SourceToComplete = SourceToAdd.Source;
                            Trace.WriteLine("Source does not exist in database, adding.", "Informational");
                        }
                        try
                        {
                            Sources.AddReference(CrawlsToComplete[i].ReferencesSourceId.Value, SourceToComplete.SourceId);
                        }
                        catch
                        {
                            Sources = new Sources();
                            Trace.WriteLine(string.Format("Source ID {0} already cites Source ID {1}", CrawlsToComplete[i].ReferencesSourceId.Value, SourceToComplete.SourceId), "Informational");
                        }
                        CrawlerProgress.CompleteQueueItem(CrawlsToComplete[i], SourceToComplete.SourceId, true);
                        Trace.WriteLine(string.Format("Dequeued and retrieved {0}/{1}, Source ID: {2}, Data-Source Specific ID: {3}", i + 1, CrawlsToComplete.Length, SourceToComplete.SourceId, CrawlsToComplete[i].DataSourceSpecificId), "Informational");
                    }
                }
                CrawlerProgress.UpdateCrawlerState(Crawl, CrawlerState.RetrievingReferencesComplete);
                Trace.WriteLine("Dequeueing references complete", "Informational");
            }
        }
    }
}
