using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATN.Data;
using System.Diagnostics;
using ATN.Crawler.WebCrawler;

namespace ATN.Crawler
{
    public class CrawlOrchestrator
    {
        private IEnumerable<CrawlSpecifier> _crawls;
        private CrawlerProgress _progress;
        private Sources _sources = new Sources();
        private SourceRetrievalService _sourceRetrieval = new SourceRetrievalService();
        public CrawlOrchestrator(IEnumerable<CrawlSpecifier> CrawlSpecifiers)
        {
            _crawls = CrawlSpecifiers;
            _progress = new CrawlerProgress();
            _sources = new Sources();
            _sourceRetrieval = new SourceRetrievalService();
        }
        public void StartCrawl()
        {
            foreach (CrawlSpecifier crawl in _crawls)
            {
                ICrawler crawler = CrawlInstantiator.InstantiateCrawler(crawl.DataSource);
                Crawl Crawl = _progress.StartCrawl(crawler.GetDataSource(), crawl.DataSourceSpecificIdentifiers.Select(id => id.ToString()).ToArray());

                //Get the canonical source from the crawler, or retrieve it from the database
                CompleteSource CanonicalSource = null;
                if (Crawl.CrawlState == (short)CrawlerState.Started)
                {
                    Trace.WriteLine("Crawl data not present, initiating crawl", "Informational");
                    Source AttachedCannonicalSource = _sourceRetrieval.GetSourceByDataSourceSpecificId(crawler.GetDataSource(), crawl.DataSourceSpecificIdentifiers.First());

                    if (AttachedCannonicalSource == null)
                    {
                        CanonicalSource = crawler.GetSourceById(crawl.DataSourceSpecificIdentifiers.First().ToString());
                        AttachedCannonicalSource = _sources.AddDetachedSource(CanonicalSource);
                    }
                    else
                    {
                        CanonicalSource = _sources.GetCompleteSourceByDataSourceSpecificId(crawler.GetDataSource(), crawl.DataSourceSpecificIdentifiers.First());
                    }

                    //If there are multiple copies of the same source added, correlate each unique data-source ID to the canonical source ID
                    foreach (string ID in crawl.DataSourceSpecificIdentifiers)
                    {
                        _progress.StoreCanonicalResult(Crawl.CrawlId, ID, CanonicalSource.Source.SourceId);
                    }

                    _progress.UpdateCrawlerState(Crawl, CrawlerState.CanonicalPaperComplete);
                    Trace.WriteLine("Canonical paper retrieved, crawl state committed", "Informational");
                }
                else
                {
                    Trace.WriteLine("Crawl already started, retrieving crawl state from database model", "Informational");

                    //Find the canonical source from the database, stopping once one is found
                    for (int i = 0; i < crawl.DataSourceSpecificIdentifiers.Length && CanonicalSource == null; i++)
                    {
                        CanonicalSource = _sources.GetCompleteSourceByDataSourceSpecificId(crawler.GetDataSource(), crawl.DataSourceSpecificIdentifiers[i]);
                    }
                }

                //Retrieve publication IDs of ciatations of the canonical paper, and add queue them for a retireval
                if (Crawl.CrawlState == (int)CrawlerState.CanonicalPaperComplete)
                {
                    Trace.WriteLine("Queueing citations", "Informational");

                    foreach (string ID in crawl.DataSourceSpecificIdentifiers)
                    {
                        //Retrieve the list of citations from data source
                        string[] PublicationIdsCitingCanonicalPaper = crawler.GetCitationsBySourceId(ID);

                        //Queue each citation as citing the canonical paper
                        _progress.QueueCrawl(Crawl.CrawlId, PublicationIdsCitingCanonicalPaper, CanonicalSource.Source.SourceId, CrawlReferenceDirection.Citation);
                        Trace.WriteLine(string.Format("Queued {0} citations", PublicationIdsCitingCanonicalPaper.Length));
                    }
                    _progress.CommitQueue();
                    _progress.UpdateCrawlerState(Crawl, CrawlerState.EnqueueingCitationsComplete);
                    Trace.WriteLine("Citation queueing complete and committed.", "Informational");
                }

                //Get all of the publication data for each queued citation, and add references between sources
                if (Crawl.CrawlState == (int)CrawlerState.EnqueueingCitationsComplete)
                {
                    Trace.WriteLine("Dequeueing and retreiving citations", "Informational");

                    //Get all of the pending citations
                    CrawlQueue[] CrawlsToComplete = _progress.GetPendingCrawlsForCrawlId(Crawl.CrawlId);
                    Trace.WriteLine(string.Format("Retreiving {0} citations", CrawlsToComplete.Length), "Informational");
                    for (int i = 0; i < CrawlsToComplete.Length; i++)
                    {
                        if (CrawlsToComplete[i].CrawlReferenceDirection == (int)CrawlReferenceDirection.Citation)
                        {
                            //See if the given data-source specific ID exists in the database, if not retrieve it from the data source and store it
                            Source SourceToComplete = _sourceRetrieval.GetSourceByDataSourceSpecificId(crawler.GetDataSource(), CrawlsToComplete[i].DataSourceSpecificId);
                            if (SourceToComplete == null)
                            {
                                CompleteSource SourceToAdd = crawler.GetSourceById(CrawlsToComplete[i].DataSourceSpecificId);
                                _sources.AddDetachedSource(SourceToAdd);
                                SourceToComplete = SourceToAdd.Source;
                                Trace.WriteLine("Source does not exist in database, adding.", "Informational");
                            }

                            try
                            {
                                //Try to add a reference between the retrieved source and the canonical paper
                                _sources.AddCitation(SourceToComplete.SourceId, CrawlsToComplete[i].ReferencesSourceId.Value);
                            }
                            catch
                            {
                                _sources = new Sources();
                                Trace.WriteLine(string.Format("Source ID {0} already cites Source ID {1}", SourceToComplete.SourceId, CrawlsToComplete[i].ReferencesSourceId.Value), "Informational");
                            }

                            //Mark this queue item as completed
                            _progress.CompleteQueueItem(CrawlsToComplete[i], SourceToComplete.SourceId);
                            Trace.WriteLine(string.Format("Dequeued and retrieved {0}/{1}, Source ID: {2}, Data-Source Specific ID: {3}", i + 1, CrawlsToComplete.Length, SourceToComplete.SourceId, CrawlsToComplete[i].DataSourceSpecificId), "Informational");
                        }
                    }
                    _progress.UpdateCrawlerState(Crawl, CrawlerState.RetrievingCitationsComplete);
                    Trace.WriteLine("Dequeueing citations complete", "Informational");
                }

                //Retrieve the publication IDs of the references for each newly-added source, and queue them for retrieval
                if (Crawl.CrawlState == (int)CrawlerState.RetrievingCitationsComplete)
                {
                    Trace.WriteLine("Queueing references", "Informational");
                    var CrawlResults = Crawl.CrawlResults.Where(cr => cr.ReferenceRetrieved == false).ToArray();
                    for (int i = 0; i < CrawlResults.Length; i++)
                    {
                        Trace.WriteLine(string.Format("Queueing references for paper {0}/{1}", i + 1, CrawlResults.Length));

                        //Get the PublicationIDs of the references for the dequeued paper
                        string[] PublicationIdsPaperReferences = crawler.GetReferencesBySourceId(CrawlResults[i].DataSourceSpecificId);

                        //Queue the retrieved publication IDs for retrieval
                        _progress.QueueCrawl(Crawl.CrawlId, PublicationIdsPaperReferences, CrawlResults[i].SourceId, CrawlReferenceDirection.Reference);

                        //Mark the dequeued publication as having it's references enumerated
                        CrawlResults[i].ReferenceRetrieved = true;

                        Trace.WriteLine(string.Format("Queued {0} references for paper {1}/{2}", PublicationIdsPaperReferences.Length, i + 1, CrawlResults.Length));
                    }
                    _progress.CommitQueue();
                    _progress.UpdateCrawlerState(Crawl, CrawlerState.EnqueueingReferencesComplete);
                    Trace.WriteLine("Reference queueing complete", "Informational");
                }

                //Get all publication data for each queued reference, and add references between sources
                if (Crawl.CrawlState == (int)CrawlerState.EnqueueingReferencesComplete)
                {
                    Trace.WriteLine("Dequeueing and retreiving references", "Informational");

                    //Get references pending retrieval
                    CrawlQueue[] CrawlsToComplete = _progress.GetPendingCrawlsForCrawlId(Crawl.CrawlId);
                    Trace.WriteLine(string.Format("Retreiving {0} queued references", CrawlsToComplete.Length), "Informational");

                    for (int i = 0; i < CrawlsToComplete.Length; i++)
                    {
                        if (CrawlsToComplete[i].CrawlReferenceDirection == (int)CrawlReferenceDirection.Reference)
                        {
                            //See if the given data-source specific ID exists in the database, if not retrieve it from the data source and store it
                            Source SourceToComplete = _sourceRetrieval.GetSourceByDataSourceSpecificId(crawler.GetDataSource(), CrawlsToComplete[i].DataSourceSpecificId);
                            if (SourceToComplete == null)
                            {
                                CompleteSource SourceToAdd = crawler.GetSourceById(CrawlsToComplete[i].DataSourceSpecificId);
                                _sources.AddDetachedSource(SourceToAdd);
                                SourceToComplete = SourceToAdd.Source;
                                Trace.WriteLine("Source does not exist in database, adding.", "Informational");
                            }
                            try
                            {
                                //Try to add a reference between the retrieved source and the canonical paper
                                _sources.AddCitation(CrawlsToComplete[i].ReferencesSourceId.Value, SourceToComplete.SourceId);
                            }
                            catch
                            {
                                _sources = new Sources();
                                Trace.WriteLine(string.Format("Source ID {0} already cites Source ID {1}", CrawlsToComplete[i].ReferencesSourceId.Value, SourceToComplete.SourceId), "Informational");
                            }

                            //Mark this queue item as completed
                            _progress.CompleteQueueItem(CrawlsToComplete[i], SourceToComplete.SourceId, true);
                            Trace.WriteLine(string.Format("Dequeued and retrieved {0}/{1}, Source ID: {2}, Data-Source Specific ID: {3}", i + 1, CrawlsToComplete.Length, SourceToComplete.SourceId, CrawlsToComplete[i].DataSourceSpecificId), "Informational");
                        }
                    }
                    _progress.UpdateCrawlerState(Crawl, CrawlerState.RetrievingReferencesComplete);
                    Trace.WriteLine("Dequeueing references complete", "Informational");

                    //To-do: Implement functionality to gather papers recently added to the tree
                }
            }
        }
    }
    public class CrawlSpecifier
    {
        public CrawlerDataSource DataSource { get; set;}
        public string[] DataSourceSpecificIdentifiers { get; set;}
    }
}
