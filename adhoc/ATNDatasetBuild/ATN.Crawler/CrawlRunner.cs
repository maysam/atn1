using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATN.Data;
using System.Diagnostics;
using ATN.Crawler.WebCrawler;

namespace ATN.Crawler
{
    public class CrawlRunner
    {
        private CrawlerProgress _progress;
        private Sources _sources = new Sources();
        public CrawlRunner()
        {
            _progress = new CrawlerProgress();
            _sources = new Sources();
        }

        public void RefreshExistingCrawls()
        {
            //This translates specific data source identifier to an ICrawler implementation capable of crawling it
            Dictionary<CrawlerDataSource, ICrawler> DataSourceToCrawler = CrawlInstantiator.RetrieveCrawlerTranslations();

            //Enumerate existing crawls
            Crawl[] ExistingCrawls = _progress.GetExistingCrawls();
            var CrawlSpecifiers = ExistingCrawls.Select(c => new { DataSource = (CrawlerDataSource)c.DataSourceId, CanonicalIds = c.CanonicalIds.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries), Crawl = c }).ToArray();
            
            foreach (var Specifier in CrawlSpecifiers)
            {
                ICrawler Crawler = DataSourceToCrawler[Specifier.DataSource];
                Source CanonicalSource = _sources.GetSourceByDataSourceSpecificIds(Specifier.DataSource, Specifier.CanonicalIds);

                if (Specifier.Crawl.CrawlState == (int)CrawlerState.Complete)
                {
                    string[] CurrentCitations = CanonicalSource.CitingSources.Select(r => r.DataSourceSpecificId).ToArray();

                    foreach (string ID in Specifier.CanonicalIds)
                    {
                        //Get the difference from stored citations and current citations from the data source
                        string[] UpdatedCitations = Crawler.GetCitationsBySourceId(CanonicalSource.DataSourceSpecificId);
                        string[] NewCitations = UpdatedCitations.Except(CurrentCitations).ToArray();

                        //Queue each citation as citing the canonical paper
                        _progress.QueueCrawl(Specifier.Crawl.CrawlId, NewCitations, CanonicalSource.SourceId, CrawlReferenceDirection.Citation);
                        Trace.WriteLine(string.Format("Queued {0} citations", NewCitations.Length));
                    }

                    _progress.CommitQueue();
                    _progress.UpdateCrawlerState(Specifier.Crawl, CrawlerState.ScheduledCrawlEnqueueingCitationsComplete);
                    Trace.WriteLine("Canonical paper retrieved, crawl state committed", "Informational");
                }

                if (Specifier.Crawl.CrawlState == (int)CrawlerState.ScheduledCrawlEnqueueingCitationsComplete)
                {
                    DequeueCitations(Crawler, Specifier.Crawl);
                    _progress.UpdateCrawlerState(Specifier.Crawl, CrawlerState.ScheduledCrawlRetrievingCitationsComplete);
                    Trace.WriteLine("Dequeueing citations complete", "Informational");
                }

                if (Specifier.Crawl.CrawlState == (int)CrawlerState.ScheduledCrawlRetrievingCitationsComplete)
                {
                    Trace.WriteLine("Queueing references for added citations");
                    var CrawlResults = Specifier.Crawl.CrawlResults.Where(cr => cr.ReferenceRetrieved == false).ToArray();

                    for (int i = 0; i < CrawlResults.Length; i++)
                    {
                        Trace.WriteLine(string.Format("Queueing references for paper {0}/{1}", i + 1, CrawlResults.Length));

                        //Get the difference from stored references and current references from data source
                        string[] CurrentReferences = CrawlResults[i].Source.References.Select(r => r.DataSourceSpecificId).ToArray();
                        string[] UpdatedReferences = Crawler.GetReferencesBySourceId(CrawlResults[i].DataSourceSpecificId);
                        string[] NewReferences = UpdatedReferences.Except(CurrentReferences).ToArray();

                        //Queue the retrieved publication IDs for retrieval
                        _progress.QueueCrawl(Specifier.Crawl.CrawlId, NewReferences, CrawlResults[i].SourceId, CrawlReferenceDirection.Reference);

                        //Mark the dequeued publication as having it's references enumerated
                        CrawlResults[i].ReferenceRetrieved = true;

                        Trace.WriteLine(string.Format("Queued {0} references for paper {1}/{2}", NewReferences.Length, i + 1, CrawlResults.Length));
                    }

                    Trace.WriteLine("Queueing references for existing citations");
                    var Citations = CanonicalSource.CitingSources.ToArray();

                    for (int i = 0; i < Citations.Length; i++)
                    {
                        string[] CurrentReferences = Citations[i].References.Select(r => r.DataSourceSpecificId).ToArray();

                        Trace.WriteLine(string.Format("Queueing references for paper {0}/{1}", i + 1, CrawlResults.Length));

                        //Get the difference from stored references and current references from data source
                        string[] UpdatedReferences = Crawler.GetReferencesBySourceId(CrawlResults[i].DataSourceSpecificId);
                        string[] NewReferences = UpdatedReferences.Except(CurrentReferences).ToArray();

                        //Queue the retrieved publication IDs for retrieval
                        _progress.QueueCrawl(Specifier.Crawl.CrawlId, NewReferences, Citations[i].SourceId, CrawlReferenceDirection.Reference);
                    }

                    _progress.CommitQueue();
                    _progress.UpdateCrawlerState(Specifier.Crawl, CrawlerState.ScheduledCrawlEnqueueingReferencesComplete);
                    Trace.WriteLine("Reference queueing complete", "Informational");
                }

                if (Specifier.Crawl.CrawlState == (int)CrawlerState.ScheduledCrawlEnqueueingReferencesComplete)
                {
                    DequeueReferences(Crawler, Specifier.Crawl);
                    _progress.UpdateCrawlerState(Specifier.Crawl, CrawlerState.Complete);
                    Trace.WriteLine("Dequeueing references complete", "Informational");
                }
            }
        }

        public void RunCrawls(IEnumerable<CrawlSpecifier> CrawlSpecifiers)
        {
            foreach (CrawlSpecifier crawl in CrawlSpecifiers)
            {
                ICrawler crawler = CrawlInstantiator.InstantiateCrawler(crawl.DataSource);
                Crawl Crawl = _progress.StartCrawl(crawler.GetDataSource(), crawl.DataSourceSpecificIdentifiers);

                //Get the canonical source from the crawler, or retrieve it from the database
                CompleteSource CanonicalSource = null;
                if (Crawl.CrawlState == (short)CrawlerState.Started)
                {
                    Trace.WriteLine("Crawl data not present, initiating crawl", "Informational");
                    Source AttachedCannonicalSource = _sources.GetSourceByDataSourceSpecificId(crawler.GetDataSource(), crawl.DataSourceSpecificIdentifiers.First());

                    //If source is null we need to retrieve it from the data source and then store it
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

                //Retrieve publication IDs of citations of the canonical paper, and add queue them for a retireval
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
                }

                //Get all of the publication data for each queued citation, and add references between sources
                if (Crawl.CrawlState == (int)CrawlerState.EnqueueingCitationsComplete)
                {
                    DequeueCitations(crawler, Crawl);
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
                    DequeueReferences(crawler, Crawl);
                    _progress.UpdateCrawlerState(Crawl, CrawlerState.Complete);
                    Trace.WriteLine("Dequeueing references complete", "Informational");
                }
            }
        }

        private void DequeueReferences(ICrawler crawler, Crawl Crawl)
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
                    Source SourceToComplete = _sources.GetSourceByDataSourceSpecificId(crawler.GetDataSource(), CrawlsToComplete[i].DataSourceSpecificId);
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
        }


        private void DequeueCitations(ICrawler crawler, Crawl Crawl)
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
                    Source SourceToComplete = _sources.GetSourceByDataSourceSpecificId(crawler.GetDataSource(), CrawlsToComplete[i].DataSourceSpecificId);
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
        }
    }
    public class CrawlSpecifier
    {
        public CrawlerDataSource DataSource { get; set;}
        public string[] DataSourceSpecificIdentifiers { get; set;}
    }
}
