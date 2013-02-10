﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ATN.Data;
using System.Diagnostics;
using ATN.Crawler.WebCrawler;

namespace ATN.Crawler
{
    //To-do: Enforce a Source not being able to cite itself... even when such citing is returned from the data source

    /// <summary>
    /// A service for initiating crawls and refreshing existing crawls
    /// </summary>
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
            RefreshExistingCrawls(null);
        }
        private void RefreshExistingCrawls(int? CrawlId = null)
        {
            //This translates specific data source identifier to an ICrawler implementation capable of crawling it
            Dictionary<CrawlerDataSource, ICrawler> DataSourceToCrawler = CrawlInstantiator.RetrieveCrawlerTranslations();

            //Enumerate existing crawls
            Crawl[] ExistingCrawls = _progress.GetExistingCrawls();
            ExistingCrawlSpecifier[] CrawlSpecifiers;
            if (CrawlId.HasValue)
            {
                CrawlSpecifiers = ExistingCrawls.Where(c => c.CrawlId == CrawlId.Value).Select(c => new ExistingCrawlSpecifier(c, c.Theory.TheoryName, c.TheoryId, c.Theory.TheoryDefinitions.ToArray())).ToArray();
            }
            else
            {
                //Prioritize crawls that haven't been run yet, then sort by how close each crawl is to completion
                CrawlSpecifiers = ExistingCrawls.Where(ec => ec.CrawlState < (int)CrawlerState.Complete).OrderByDescending(ec => ec.CrawlState).OrderBy(ec => ec.DateCrawled).Select(c => new ExistingCrawlSpecifier(c, c.Theory.TheoryName, c.TheoryId, c.Theory.TheoryDefinitions.ToArray())).ToArray();
                CrawlSpecifiers = CrawlSpecifiers.Union(ExistingCrawls.Where(ec => ec.CrawlState >= (int)CrawlerState.Complete).OrderByDescending(ec => ec.CrawlState).OrderBy(ec => ec.DateCrawled).Select(c => new ExistingCrawlSpecifier(c, c.Theory.TheoryName, c.TheoryId, c.Theory.TheoryDefinitions.ToArray()))).ToArray();
            }

            Trace.WriteLine(string.Format("Refreshing {0} crawls", CrawlSpecifiers.Length), "Informational");

            foreach (var Specifier in CrawlSpecifiers)
            {
                Dictionary<Source, CanonicalDataSource> CanonicalSources = new Dictionary<Source, CanonicalDataSource>(Specifier.CanonicalDataSources.Length);

                if (Specifier.Crawl.CrawlState == (short)CrawlerState.Started)
                {
                    foreach (CanonicalDataSource CanonicalDataSource in Specifier.CanonicalDataSources)
                    {
                        ICrawler Crawler = DataSourceToCrawler[CanonicalDataSource.DataSource];

                        Trace.WriteLine("Crawl data not present, initiating crawl", "Informational");
                        Source AttachedCannonicalSource = _sources.GetSourceByDataSourceSpecificId(Crawler.GetDataSource(), CanonicalDataSource.CanonicalIds.First());

                        //If source is null we need to retrieve it from the data source and then store it
                        if (AttachedCannonicalSource == null)
                        {
                            CompleteSource CanonicalCompleteSource = Crawler.GetSourceById(CanonicalDataSource.CanonicalIds.First().ToString());
                            AttachedCannonicalSource = _sources.AddDetachedSource(CanonicalCompleteSource);
                        }

                        //If there are multiple copies of the same source added, correlate each unique data-source ID to the canonical source ID
                        foreach (string ID in CanonicalDataSource.CanonicalIds)
                        {
                            _progress.StoreCanonicalResult(Specifier.Crawl.CrawlId, ID, AttachedCannonicalSource.SourceId);
                        }
                        CanonicalSources.Add(AttachedCannonicalSource, CanonicalDataSource);
                    }
                    _progress.UpdateCrawlerState(Specifier.Crawl, CrawlerState.CanonicalPaperComplete);
                    Trace.WriteLine("Canonical paper retrieved, crawl state committed", "Informational");
                }
                else
                {
                    Trace.WriteLine("Crawl already started, retrieving crawl state from database model", "Informational");
                    //Find the canonical source from the database, stopping once one is found
                    foreach (CanonicalDataSource CanonicalDataSource in Specifier.CanonicalDataSources)
                    {
                        CanonicalSources.Add(_sources.GetSourceByDataSourceSpecificIds(CanonicalDataSource.DataSource, CanonicalDataSource.CanonicalIds), CanonicalDataSource);
                    }
                }
                if (Specifier.Crawl.CrawlState == (int)CrawlerState.ScheduledCrawlStarted)
                {
                    Trace.WriteLine(string.Format("Removing existing queue items for Crawl {0}", Specifier.Crawl.CrawlId), "Informational");

                    //This means the enumeration of citations was interrupted; remove them and start over
                    _progress.RemoveInterruptedQueueItems(Specifier.Crawl.CrawlId);
                    _progress.UpdateCrawlerState(Specifier.Crawl, CrawlerState.Complete);
                }
                if (Specifier.Crawl.CrawlState == (int)CrawlerState.Complete || Specifier.Crawl.CrawlState == (int)CrawlerState.CanonicalPaperComplete)
                {
                    Trace.WriteLine(string.Format("Refreshing citations for Crawl {0}", Specifier.Crawl.CrawlId), "Informational");

                    _progress.UpdateCrawlerState(Specifier.Crawl, CrawlerState.ScheduledCrawlStarted);

                    foreach (KeyValuePair<Source, CanonicalDataSource> CanonicalSourceAndLocalSource in CanonicalSources)
                    {
                        Source LocalSource = CanonicalSourceAndLocalSource.Key;
                        CanonicalDataSource CanonicalSource = CanonicalSourceAndLocalSource.Value;

                        ICrawler Crawler = DataSourceToCrawler[CanonicalSource.DataSource];

                        string[] CurrentCitations = LocalSource.CitingSources.Select(r => r.DataSourceSpecificId).ToArray();

                        foreach (string ID in CanonicalSource.CanonicalIds)
                        {
                            Trace.WriteLine(string.Format("Refreshing citations for ID {0}, data source {1}", ID, CanonicalSource.DataSource), "Informational");

                            //Get the difference from stored citations and current citations from the data source
                            string[] UpdatedCitations = Crawler.GetCitationsBySourceId(ID);
                            string[] NewCitations = UpdatedCitations.Except(CurrentCitations).ToArray();

                            //Queue each citation as citing the canonical paper
                            _progress.QueueReferenceCrawl(Specifier.Crawl.CrawlId, NewCitations, CanonicalSource.DataSource, LocalSource.SourceId, CrawlReferenceDirection.Citation);
                            Trace.WriteLine(string.Format("Queued {0} citations", NewCitations.Length));
                        }
                    }

                    _progress.CommitQueue();
                    _progress.UpdateCrawlerState(Specifier.Crawl, (CrawlerState)(Specifier.Crawl.CrawlState + 1));
                    Trace.WriteLine("Canonical paper retrieved, crawl state committed", "Informational");
                }

                if (Specifier.Crawl.CrawlState == (int)CrawlerState.ScheduledCrawlEnqueueingCitationsComplete || Specifier.Crawl.CrawlState == (int)CrawlerState.EnqueueingCitationsComplete)
                {
                    DequeueCitations(Specifier.Crawl);
                    _progress.UpdateCrawlerState(Specifier.Crawl, (CrawlerState)(Specifier.Crawl.CrawlState + 1));
                    Trace.WriteLine("Dequeueing citations complete", "Informational");
                }

                if (Specifier.Crawl.CrawlState == (int)CrawlerState.ScheduledCrawlRetrievingCitationsComplete || Specifier.Crawl.CrawlState == (int)CrawlerState.RetrievingCitationsComplete)
                {
                    Trace.WriteLine("Queueing references for existing citations");

                    foreach (KeyValuePair<Source, CanonicalDataSource> CanonicalSourceAndLocalSource in CanonicalSources)
                    {
                        Source LocalSource = CanonicalSourceAndLocalSource.Key;
                        CanonicalDataSource CanonicalSource = CanonicalSourceAndLocalSource.Value;
                        ICrawler Crawler = DataSourceToCrawler[CanonicalSource.DataSource];

                        //For instances where the crawl may be interrupted, get sources starting at the last referenced source id + 1
                        //On resume, references will be queued starting from the next source rather than the beginning
                        long LastReferencedSourceId = _progress.GetLastSourceIdReferencedInCrawl(Specifier.Crawl.CrawlId);
                        var Citations = LocalSource.CitingSources.Where(c => c.SourceId > LastReferencedSourceId).OrderBy(c => c.SourceId).ToArray();

                        for (int i = 0; i < Citations.Length; i++)
                        {
                            string[] CurrentReferences = Citations[i].References.Select(r => r.DataSourceSpecificId).ToArray();

                            Trace.WriteLine(string.Format("Queueing references for paper {0}/{1}", i + 1, Citations.Length));

                            //Get the difference from stored references and current references from data source
                            string[] UpdatedReferences = Crawler.GetReferencesBySourceId(Citations[i].DataSourceSpecificId);
                            string[] NewReferences = UpdatedReferences.Except(CurrentReferences).ToArray();

                            Trace.WriteLine(string.Format("Found {0} new references", NewReferences.Length));

                            //Queue the retrieved publication IDs for retrieval
                            _progress.QueueReferenceCrawl(Specifier.Crawl.CrawlId, NewReferences, CanonicalSource.DataSource, Citations[i].SourceId, CrawlReferenceDirection.Reference);
                            _progress.CommitQueue();

                            _progress.UpdateCrawlerLastEnumeratedSource(Specifier.Crawl, Citations[i].SourceId);
                        }
                    }

                    Trace.WriteLine("Queueing references for added citations");
                    var CrawlResults = Specifier.Crawl.CrawlResults.Where(cr => cr.ReferenceRetrieved == false).ToArray();

                    for (int i = 0; i < CrawlResults.Length; i++)
                    {
                        ICrawler Crawler = DataSourceToCrawler[(CrawlerDataSource)CrawlResults[i].DataSourceId];
                        Trace.WriteLine(string.Format("Queueing references for paper {0}/{1}", i + 1, CrawlResults.Length));

                        //Get the difference from stored references and current references from data source
                        string[] CurrentReferences = CrawlResults[i].Source.References.Select(r => r.DataSourceSpecificId).ToArray();
                        string[] UpdatedReferences = Crawler.GetReferencesBySourceId(CrawlResults[i].DataSourceSpecificId);
                        string[] NewReferences = UpdatedReferences.Except(CurrentReferences).ToArray();

                        //Queue the retrieved publication IDs for retrieval
                        _progress.QueueReferenceCrawl(Specifier.Crawl.CrawlId, NewReferences, (CrawlerDataSource)CrawlResults[i].DataSourceId, CrawlResults[i].SourceId, CrawlReferenceDirection.Reference);

                        //Mark the dequeued publication as having it's references enumerated
                        CrawlResults[i].ReferenceRetrieved = true;

                        Trace.WriteLine(string.Format("Queued {0} references for paper {1}/{2}", NewReferences.Length, i + 1, CrawlResults.Length));
                    }

                    _progress.CommitQueue();
                    _progress.UpdateCrawlerState(Specifier.Crawl, (CrawlerState)(Specifier.Crawl.CrawlState + 1));
                    Trace.WriteLine("Reference queueing complete", "Informational");
                }

                if (Specifier.Crawl.CrawlState == (int)CrawlerState.ScheduledCrawlEnqueueingReferencesComplete || Specifier.Crawl.CrawlState == (int)CrawlerState.EnqueueingReferencesComplete)
                {
                    DequeueReferences(Specifier.Crawl);
                    _progress.UpdateCrawlerState(Specifier.Crawl, CrawlerState.Complete);
                    Trace.WriteLine("Dequeueing references complete", "Informational");
                }
            }
        }
        public void StartNewCrawl(NewCrawlSpecifier CrawlSpecifier)
        {
            Theories Theories = new Theories();
            Theory TheoryToCrawl = Theories.AddTheory(CrawlSpecifier.TheoryName, CrawlSpecifier.CanonicalDataSources);
            PendingCrawlSpecifier pcs = new PendingCrawlSpecifier(TheoryToCrawl.TheoryId, CrawlSpecifier);
            Crawl Crawl = _progress.QueueTheoryCrawl(pcs);
            Trace.WriteLine(string.Format("Queueing crawl using for theory {0}", TheoryToCrawl.TheoryName, "Informational"));
            RefreshExistingCrawls(Crawl.CrawlId);
        }

        private void DequeueReferences(Crawl Crawl)
        {
            Dictionary<CrawlerDataSource, ICrawler> DataSourceToCrawler = CrawlInstantiator.RetrieveCrawlerTranslations();
            Trace.WriteLine("Dequeueing and retreiving references", "Informational");

            //Get references pending retrieval
            CrawlQueue[] CrawlsToComplete = _progress.GetPendingCrawlsForCrawlId(Crawl.CrawlId);
            Trace.WriteLine(string.Format("Retreiving {0} queued references", CrawlsToComplete.Length), "Informational");

            for (int i = 0; i < CrawlsToComplete.Length; i++)
            {
                ICrawler CrawlerForDataSource = DataSourceToCrawler[(CrawlerDataSource)CrawlsToComplete[i].DataSourceId];
                if (CrawlsToComplete[i].CrawlReferenceDirection == (int)CrawlReferenceDirection.Reference)
                {
                    //See if the given data-source specific ID exists in the database, if not retrieve it from the data source and store it
                    Source SourceToComplete = _sources.GetSourceByDataSourceSpecificId(CrawlerForDataSource.GetDataSource(), CrawlsToComplete[i].DataSourceSpecificId);
                    if (SourceToComplete == null)
                    {
                        CompleteSource SourceToAdd = CrawlerForDataSource.GetSourceById(CrawlsToComplete[i].DataSourceSpecificId);
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


        private void DequeueCitations(Crawl Crawl)
        {
            Dictionary<CrawlerDataSource, ICrawler> DataSourceToCrawler = CrawlInstantiator.RetrieveCrawlerTranslations();
            Trace.WriteLine("Dequeueing and retreiving citations", "Informational");

            //Get all of the pending citations
            CrawlQueue[] CrawlsToComplete = _progress.GetPendingCrawlsForCrawlId(Crawl.CrawlId);
            Trace.WriteLine(string.Format("Retreiving {0} citations", CrawlsToComplete.Length), "Informational");
            for (int i = 0; i < CrawlsToComplete.Length; i++)
            {
                ICrawler CrawlerForDataSource = DataSourceToCrawler[(CrawlerDataSource)CrawlsToComplete[i].DataSourceId];

                if (CrawlsToComplete[i].CrawlReferenceDirection == (int)CrawlReferenceDirection.Citation)
                {
                    //See if the given data-source specific ID exists in the database, if not retrieve it from the data source and store it
                    Source SourceToComplete = _sources.GetSourceByDataSourceSpecificId(CrawlerForDataSource.GetDataSource(), CrawlsToComplete[i].DataSourceSpecificId);
                    if (SourceToComplete == null)
                    {
                        CompleteSource SourceToAdd = CrawlerForDataSource.GetSourceById(CrawlsToComplete[i].DataSourceSpecificId);
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
}
