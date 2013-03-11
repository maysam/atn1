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
        private Sources _sources;
        private Theories _theories;
        public CrawlRunner(ATNEntities Entities = null)
        {
            _progress = new CrawlerProgress(Entities);
            _sources = new Sources(Entities);
            _theories = new Theories(Entities);
        }

        /// <summary>
        /// Enumerates all crawls, finishing incomplete ones and recrawling stale ones
        /// </summary>
        public ExistingCrawlSpecifier[] ProcessCurrentCrawls()
        {
            List<ExistingCrawlSpecifier> ChangedCrawls = new List<ExistingCrawlSpecifier>();

            Crawl[] ExistingCrawls = _progress.GetExistingCrawls();
            ExistingCrawlSpecifier[] CrawlSpecifiers = ExistingCrawls.Where(c => c.CrawlState < 5).OrderByDescending(c => c.CrawlState).Select(c => new ExistingCrawlSpecifier(c, c.Theory.TheoryName, c.Theory.TheoryComment, c.TheoryId, c.Theory.TheoryDefinitions.ToArray())).ToArray();
            CrawlSpecifiers = CrawlSpecifiers.Union(ExistingCrawls.Where(c => c.CrawlState > 5).OrderByDescending(c => c.CrawlState).Select(c => new ExistingCrawlSpecifier(c, c.Theory.TheoryName, c.Theory.TheoryComment, c.TheoryId, c.Theory.TheoryDefinitions.ToArray()))).ToArray();
            CrawlSpecifiers = CrawlSpecifiers.Union(ExistingCrawls.Where(c => c.CrawlState == 5 && c.CrawlIntervalDays.HasValue && c.DateCrawled <= DateTime.Now.AddDays(-c.CrawlIntervalDays.Value)).Select(c => new ExistingCrawlSpecifier(c, c.Theory.TheoryName, c.Theory.TheoryComment, c.TheoryId, c.Theory.TheoryDefinitions.ToArray()))).ToArray();
            foreach (ExistingCrawlSpecifier Specifier in CrawlSpecifiers)
            {
                if (RefreshExistingCrawls(Specifier.Crawl.CrawlId))
                {
                    ChangedCrawls.Add(Specifier);
                }
            }

            return ChangedCrawls.ToArray();
        }

        /// <summary>
        /// Refreshes all existing crawls, or a single crawl indicated by CrawlId
        /// </summary>
        /// <param name="CrawlId">CrawlId to refresh, if this is null all existing crawls will be refreshed</param>
        private bool RefreshExistingCrawls(int? CrawlId = null)
        {
            bool Changed = false;

            //This translates specific data source identifier to an ICrawler implementation capable of crawling it
            Dictionary<CrawlerDataSource, ICrawler> DataSourceToCrawler = CrawlInstantiator.RetrieveCrawlerTranslations();

            //Enumerate existing crawls
            Crawl[] ExistingCrawls = _progress.GetExistingCrawls();
            ExistingCrawlSpecifier[] CrawlSpecifiers;
            if (CrawlId.HasValue)
            {
                CrawlSpecifiers = ExistingCrawls.Where(c => c.CrawlId == CrawlId.Value).Select(c => new ExistingCrawlSpecifier(c, c.Theory.TheoryName, c.Theory.TheoryComment, c.TheoryId, c.Theory.TheoryDefinitions.ToArray())).ToArray();
            }
            else
            {
                //Prioritize crawls that haven't been run yet, then sort by how close each crawl is to completion
                CrawlSpecifiers = ExistingCrawls.Where(ec => ec.CrawlState < (int)CrawlerState.Complete).OrderByDescending(ec => ec.CrawlState).OrderBy(ec => ec.DateCrawled).Select(c => new ExistingCrawlSpecifier(c, c.Theory.TheoryName, c.Theory.TheoryComment, c.TheoryId, c.Theory.TheoryDefinitions.ToArray())).ToArray();
                CrawlSpecifiers = CrawlSpecifiers.Union(ExistingCrawls.Where(ec => ec.CrawlState >= (int)CrawlerState.Complete).OrderByDescending(ec => ec.CrawlState).Select(c => new ExistingCrawlSpecifier(c, c.Theory.TheoryName, c.Theory.TheoryComment, c.TheoryId, c.Theory.TheoryDefinitions.ToArray()))).ToArray();
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
                            _progress.StoreCanonicalResult(Specifier.Crawl.CrawlId, ID, CanonicalDataSource.DataSource, AttachedCannonicalSource.SourceId);
                        }

                        CanonicalSources.Add(AttachedCannonicalSource, CanonicalDataSource);
                    }
                    Changed = true;
                    _progress.UpdateCrawlerState(Specifier.Crawl, CrawlerState.CanonicalPaperComplete);
                    Trace.WriteLine("Canonical papers retrieved, crawl state committed", "Informational");
                }
                else
                {
                    Trace.WriteLine("Crawl already started, retrieving crawl state from database model", "Informational");
                    //Find the canonical sources from the database
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

                    _progress.UpdateCrawlerState(Specifier.Crawl, (CrawlerState)(Specifier.Crawl.CrawlState + 1));

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

                            if (NewCitations.Length > 0)
                            {
                                Changed = true;
                            }
                        }
                    }

                    _progress.CommitQueue();
                    _progress.UpdateCrawlerState(Specifier.Crawl, (CrawlerState)(Specifier.Crawl.CrawlState + 1));
                    Trace.WriteLine("Canonical paper retrieved, crawl state committed", "Informational");
                }

                if (Specifier.Crawl.CrawlState == (int)CrawlerState.ScheduledCrawlEnqueueingCitationsComplete || Specifier.Crawl.CrawlState == (int)CrawlerState.EnqueueingCitationsComplete)
                {
                    if (DequeueCitationsReferences(Specifier.Crawl))
                    {
                        Changed = true;
                    }
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

                            if (NewReferences.Length > 0)
                            {
                                Changed = true;
                            }

                            Trace.WriteLine(string.Format("Found {0} new references", NewReferences.Length));

                            //Queue the retrieved publication IDs for retrieval
                            _progress.QueueReferenceCrawl(Specifier.Crawl.CrawlId, NewReferences, CanonicalSource.DataSource, Citations[i].SourceId, CrawlReferenceDirection.Reference);
                            _progress.CommitQueue();

                            _progress.UpdateCrawlerLastEnumeratedSource(Specifier.Crawl, Citations[i].SourceId);
                        }
                    }

                    _progress.CommitQueue();
                    _progress.UpdateCrawlerState(Specifier.Crawl, (CrawlerState)(Specifier.Crawl.CrawlState + 1));
                    Trace.WriteLine("Reference queueing complete", "Informational");
                }

                if (Specifier.Crawl.CrawlState == (int)CrawlerState.ScheduledCrawlEnqueueingReferencesComplete || Specifier.Crawl.CrawlState == (int)CrawlerState.EnqueueingReferencesComplete)
                {
                    if (DequeueCitationsReferences(Specifier.Crawl))
                    {
                        Changed = true;
                    }
                    _progress.UpdateCrawlerState(Specifier.Crawl, CrawlerState.Complete);
                    Trace.WriteLine("Dequeueing references complete", "Informational");
                }
            }
            return Changed;
        }

        /// <summary>
        /// Queue a crawl for a new theory
        /// </summary>
        /// <param name="CrawlSpecifier">The parameters of the theory being crawled</param>
        /// <param name="CrawlIntervalDays">The interval, in days, between refreshes of this crawl. If null, this theory will not be refreshed.</param>
        public void StartNewCrawl(NewCrawlSpecifier CrawlSpecifier, int? CrawlIntervalDays = null)
        {
            Theory TheoryToCrawl = _theories.AddTheory(CrawlSpecifier.TheoryName, CrawlSpecifier.TheoryComment, CrawlSpecifier.CanonicalDataSources);
            PendingCrawlSpecifier pcs = new PendingCrawlSpecifier(TheoryToCrawl.TheoryId, CrawlSpecifier, CrawlIntervalDays);
            Crawl Crawl = _progress.QueueTheoryCrawl(pcs);
            Trace.WriteLine(string.Format("Queueing crawl using for theory {0}", TheoryToCrawl.TheoryName, "Informational"));
        }

        /// <summary>
        /// Dequeues existing crawl-specific queue items for citations and references
        /// </summary>
        /// <param name="Crawl">The crawl to dequeue items for</param>
        /// <returns>True if there were any citations or references dequeued; false otherwise</returns>
        private bool DequeueCitationsReferences(Crawl Crawl)
        {
            Dictionary<CrawlerDataSource, ICrawler> DataSourceToCrawler = CrawlInstantiator.RetrieveCrawlerTranslations();
            Trace.WriteLine("Dequeueing and retreiving references", "Informational");

            //Get references pending retrieval
            CrawlQueue[] CrawlsToComplete = _progress.GetPendingCrawlsForCrawlId(Crawl.CrawlId);
            Trace.WriteLine(string.Format("Retreiving {0} queued references", CrawlsToComplete.Length), "Informational");

            for (int i = 0; i < CrawlsToComplete.Length; i++)
            {
                ICrawler CrawlerForDataSource = DataSourceToCrawler[(CrawlerDataSource)CrawlsToComplete[i].DataSourceId];

                //See if the given data-source specific ID exists in the database, if not retrieve it from the data source and store it
                Source SourceToComplete = _sources.GetSourceByDataSourceSpecificId(CrawlerForDataSource.GetDataSource(), CrawlsToComplete[i].DataSourceSpecificId);
                if (SourceToComplete == null)
                {
                    CompleteSource SourceToAdd = CrawlerForDataSource.GetSourceById(CrawlsToComplete[i].DataSourceSpecificId);
                    SourceToComplete = _sources.AddDetachedSource(SourceToAdd);

                    Trace.WriteLine("Source does not exist in database, adding.", "Informational");
                }
                try
                {
                    //Add a citation between the retrieved source and the paper which it cites
                    //The reference direction merely switches the direction of the citation,
                    //which merely swaps the parameters to the AddCitation call
                    if (CrawlsToComplete[i].CrawlReferenceDirection == (int)CrawlReferenceDirection.Reference)
                    {
                        _sources.AddCitation(CrawlsToComplete[i].ReferencesSourceId.Value, SourceToComplete.SourceId);
                    }
                    else if (CrawlsToComplete[i].CrawlReferenceDirection == (int)CrawlReferenceDirection.Citation)
                    {
                        _sources.AddCitation(SourceToComplete.SourceId, CrawlsToComplete[i].ReferencesSourceId.Value);
                    }
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

            if (CrawlsToComplete.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}