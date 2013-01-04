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
        static uint[] DataSourceSpecificCanonicalIds = new uint[] { 44554959 };

        static void Main(string[] args)
        {
            CrawlerProgress CrawlerProgress = new CrawlerProgress();
            Crawl Crawl = CrawlerProgress.StartCrawl(CrawlerDataSource.MicrosoftAcademicSearch, DataSourceSpecificCanonicalIds.Select(id => id.ToString()).ToArray());

            ICrawler crawler = new MASCrawler();
            Sources Sources = new Sources();
            SourceRetrievalService SourceRetrieval = new SourceRetrievalService();

            //Get the canonical source from the crawler, or retrieve it from the database
            CompleteSource CanonicalSource = null;
            if (Crawl.CrawlState == (short)CrawlerState.Started)
            {
                Trace.WriteLine("Crawl data not present, initiating crawl", "Informational");
                Source AttachedCannonicalSource = SourceRetrieval.GetSourceByDataSourceSpecificId(CrawlerDataSource.MicrosoftAcademicSearch, DataSourceSpecificCanonicalIds.First().ToString());

                if (AttachedCannonicalSource == null)
                {
                    CanonicalSource = crawler.GetSourceById(DataSourceSpecificCanonicalIds.First().ToString());
                    AttachedCannonicalSource = Sources.AddDetachedSource(CanonicalSource);
                }
                else
                {
                    CanonicalSource = Sources.GetCompleteSourceByDataSourceSpecificId(CrawlerDataSource.MicrosoftAcademicSearch, AttachedCannonicalSource.MasID.Value.ToString());
                }

                //If there are multiple copies of the same source added, correlate each unique data-source ID to the canonical source ID
                foreach (uint MasId in DataSourceSpecificCanonicalIds)
                {
                    CrawlerProgress.StoreCanonicalResult(Crawl.CrawlId, MasId.ToString(), CanonicalSource.Source.SourceId);
                }

                CrawlerProgress.UpdateCrawlerState(Crawl, CrawlerState.CanonicalPaperComplete);
                Trace.WriteLine("Canonical paper retrieved, crawl state committed", "Informational");
            }
            else
            {
                Trace.WriteLine("Crawl already started, retrieving crawl state from database model", "Informational");

                //Find the canonical source from the database, stopping once one is found
                for (int i = 0; i < DataSourceSpecificCanonicalIds.Length && CanonicalSource == null; i++)
                {
                    CanonicalSource = Sources.GetCompleteSourceByDataSourceSpecificId(CrawlerDataSource.MicrosoftAcademicSearch, DataSourceSpecificCanonicalIds[i].ToString());
                }
            }

            //Retrieve publication IDs of ciatations of the canonical paper, and add queue them for a retireval
            if(Crawl.CrawlState == (int)CrawlerState.CanonicalPaperComplete)
            {
                Trace.WriteLine("Queueing citations", "Informational");

                foreach (uint MasId in DataSourceSpecificCanonicalIds)
                {
                    //Retrieve the list of citations from data source
                    string[] PublicationIdsCitingCanonicalPaper = crawler.GetCitationsBySourceId(MasId.ToString());

                    //Queue each citation as citing the canonical paper
                    CrawlerProgress.QueueCrawl(Crawl.CrawlId, PublicationIdsCitingCanonicalPaper, CanonicalSource.Source.SourceId, CrawlReferenceDirection.Citation);
                    Trace.WriteLine(string.Format("Queued {0} citations", PublicationIdsCitingCanonicalPaper.Length));
                }

                CrawlerProgress.UpdateCrawlerState(Crawl, CrawlerState.EnqueueingCitationsComplete);
                Trace.WriteLine("Citation queueing complete", "Informational");
            }

            //Get all of the publication data for each queued citation, and add references between sources
            if (Crawl.CrawlState == (int)CrawlerState.EnqueueingCitationsComplete)
            {
                Trace.WriteLine("Dequeueing and retreiving citations", "Informational");

                //Get all of the pending citations
                CrawlQueue[] CrawlsToComplete = CrawlerProgress.GetPendingCrawlsForCrawlId(Crawl.CrawlId);
                Trace.WriteLine(string.Format("Retreiving {0} citations", CrawlsToComplete.Length), "Informational");
                for(int i = 0; i < CrawlsToComplete.Length; i++)
                {
                    if(CrawlsToComplete[i].CrawlReferenceDirection == (int)CrawlReferenceDirection.Citation)
                    {
                        //See if the given data-source specific ID exists in the database, if not retrieve it from the data source and store it
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
                            //Try to add a reference between the retrieved source and the canonical paper
                            Sources.AddCitation(SourceToComplete.SourceId, CrawlsToComplete[i].ReferencesSourceId.Value);
                        }
                        catch
                        {
                            Sources = new Sources();
                            Trace.WriteLine(string.Format("Source ID {0} already cites Source ID {1}", SourceToComplete.SourceId, CrawlsToComplete[i].ReferencesSourceId.Value), "Informational");
                        }
                        
                        //Mark this queue item as completed
                        CrawlerProgress.CompleteQueueItem(CrawlsToComplete[i], SourceToComplete.SourceId);
                        Trace.WriteLine(string.Format("Dequeued and retrieved {0}/{1}, Source ID: {2}, Data-Source Specific ID: {3}", i + 1, CrawlsToComplete.Length, SourceToComplete.SourceId, CrawlsToComplete[i].DataSourceSpecificId), "Informational");
                    }
                }
                CrawlerProgress.UpdateCrawlerState(Crawl, CrawlerState.RetrievingCitationsComplete);
                Trace.WriteLine("Dequeueing citations complete", "Informational");
            }

            //Retrieve the publication IDs of the references for each newly-added source, and queue them for retrieval
            if (Crawl.CrawlState == (int)CrawlerState.RetrievingCitationsComplete)
            {
                Trace.WriteLine("Queueing references", "Informational");
                var CrawlResults = Crawl.CrawlResults.Where(cr => cr.ReferencesRetrieved == false).ToArray();
                for (int i = 0; i < CrawlResults.Length; i++)
                {
                    Trace.WriteLine(string.Format("Queueing references for paper {0}/{1}", i + 1, CrawlResults.Length));

                    //Get the PublicationIDs of the references for the dequeued paper
                    string[] PublicationIdsPaperReferences = crawler.GetReferencesBySourceId(CrawlResults[i].DataSourceSpecificId);

                    //Queue the retrieved publication IDs for retrieval
                    CrawlerProgress.QueueCrawl(Crawl.CrawlId, PublicationIdsPaperReferences, CrawlResults[i].SourceId, CrawlReferenceDirection.Reference);

                    //Mark the dequeued publication as having it's referenced enumerated
                    CrawlResults[i].ReferencesRetrieved = true;
                    CrawlerProgress.Context.SaveChanges();

                    Trace.WriteLine(string.Format("Queued {0} references for paper {1}/{2}", PublicationIdsPaperReferences.Length, i + 1, CrawlResults.Length));
                }
                CrawlerProgress.UpdateCrawlerState(Crawl, CrawlerState.EnqueueingReferencesComplete);
                Trace.WriteLine("Reference queueing complete", "Informational");
            }

            //Get all publication data for each queued reference, and add references between sources
            if (Crawl.CrawlState == (int)CrawlerState.EnqueueingReferencesComplete)
            {
                Trace.WriteLine("Dequeueing and retreiving references", "Informational");

                //Get references pending retrieval
                CrawlQueue[] CrawlsToComplete = CrawlerProgress.GetPendingCrawlsForCrawlId(Crawl.CrawlId);
                Trace.WriteLine(string.Format("Retreiving {0} queued references", CrawlsToComplete.Length), "Informational");

                for (int i = 0; i < CrawlsToComplete.Length; i++)
                {
                    if (CrawlsToComplete[i].CrawlReferenceDirection == (int)CrawlReferenceDirection.Reference)
                    {
                        //See if the given data-source specific ID exists in the database, if not retrieve it from the data source and store it
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
                            //Try to add a reference between the retrieved source and the canonical paper
                            Sources.AddCitation(CrawlsToComplete[i].ReferencesSourceId.Value, SourceToComplete.SourceId);
                        }
                        catch
                        {
                            Sources = new Sources();
                            Trace.WriteLine(string.Format("Source ID {0} already cites Source ID {1}", CrawlsToComplete[i].ReferencesSourceId.Value, SourceToComplete.SourceId), "Informational");
                        }

                        //Mark this queue item as completed
                        CrawlerProgress.CompleteQueueItem(CrawlsToComplete[i], SourceToComplete.SourceId, true);
                        Trace.WriteLine(string.Format("Dequeued and retrieved {0}/{1}, Source ID: {2}, Data-Source Specific ID: {3}", i + 1, CrawlsToComplete.Length, SourceToComplete.SourceId, CrawlsToComplete[i].DataSourceSpecificId), "Informational");
                    }
                }
                CrawlerProgress.UpdateCrawlerState(Crawl, CrawlerState.RetrievingReferencesComplete);
                Trace.WriteLine("Dequeueing references complete", "Informational");

                //Complete initial crawl
                //To-do: Implement functionality to gather papers recently added to the tree
            }
        }
    }
}
