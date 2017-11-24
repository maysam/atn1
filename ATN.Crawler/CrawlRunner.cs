using System;
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
    public class CrawlRunner : IDisposable
    {
        private ATNEntities _context;
        private CrawlerProgress _progress;
        private Sources _sources;
        private Theories _theories;
        public CrawlRunner(ATNEntities Entities = null)
        {
            if (Entities == null)
            {
                _context = new ATNEntities();
            }
            else
            {
                _context = Entities;
            }
            _progress = new CrawlerProgress(_context);
            _sources = new Sources(_context);
            _theories = new Theories(_context);
        }

        public void Dispose()
        {
            _sources.Dispose();
            _progress.Dispose();
            _theories.Dispose();
            _context.Dispose();

            _context = null;
            _sources = null;
            _theories = null;
            _progress = null;
        }

        /// <summary>
        /// Enumerates all crawls, finishing incomplete ones and recrawling stale ones
        /// </summary>
        public ExistingCrawlSpecifier[] ProcessCurrentCrawls()
        {
            List<ExistingCrawlSpecifier> ChangedCrawls = new List<ExistingCrawlSpecifier>();

            Crawl[] ExistingCrawls = _progress.GetExistingCrawls();
            ExistingCrawlSpecifier[] CrawlSpecifiers = ExistingCrawls.Where(c => c.CrawlState < 5).OrderByDescending(c => c.CrawlState).Select(c => new ExistingCrawlSpecifier(c, c.Theory.TheoryName, c.Theory.TheoryComment, c.TheoryId, c.Theory.ArticleLevelEigenfactor, c.Theory.ImpactFactor, c.Theory.TheoryAttributionRatio, c.Theory.DataMining, c.Theory.Clustering, c.Theory.TheoryDefinitions.ToArray())).ToArray();
            CrawlSpecifiers = CrawlSpecifiers.Union(ExistingCrawls.Where(c => c.CrawlState > 5).OrderByDescending(c => c.CrawlState).Select(c => new ExistingCrawlSpecifier(c, c.Theory.TheoryName, c.Theory.TheoryComment, c.TheoryId, c.Theory.ArticleLevelEigenfactor, c.Theory.ImpactFactor, c.Theory.TheoryAttributionRatio, c.Theory.DataMining, c.Theory.Clustering, c.Theory.TheoryDefinitions.ToArray()))).ToArray();
            CrawlSpecifiers = CrawlSpecifiers.Union(ExistingCrawls.Where(c => c.CrawlState == 5 && c.CrawlIntervalDays.HasValue && c.DateCrawled <= DateTime.Now.AddDays(-c.CrawlIntervalDays.Value)).Select(c => new ExistingCrawlSpecifier(c, c.Theory.TheoryName, c.Theory.TheoryComment, c.TheoryId, c.Theory.ArticleLevelEigenfactor, c.Theory.ImpactFactor, c.Theory.TheoryAttributionRatio, c.Theory.DataMining, c.Theory.Clustering, c.Theory.TheoryDefinitions.ToArray()))).ToArray();
            
            foreach (ExistingCrawlSpecifier Specifier in CrawlSpecifiers)
            {
                if (RefreshExistingCrawl(Specifier.Crawl.CrawlId))
                {
                    ChangedCrawls.Add(Specifier);
                }
            }

            ChangedCrawls = ChangedCrawls.Union(ExistingCrawls.Where(c => c.HasChanged).Select(c => new ExistingCrawlSpecifier(c, c.Theory.TheoryName, c.Theory.TheoryComment, c.TheoryId, c.Theory.ArticleLevelEigenfactor, c.Theory.ImpactFactor, c.Theory.TheoryAttributionRatio, c.Theory.DataMining, c.Theory.Clustering, c.Theory.TheoryDefinitions.ToArray()))).ToList();

            return ChangedCrawls.ToArray();
        }

        /// <summary>
        /// Refreshes a single crawl indicated by CrawlId
        /// </summary>
        /// <param name="CrawlId">CrawlId to refresh</param>
        private bool RefreshExistingCrawl(int CrawlId)
        {
            //This translates specific data source identifier to an ICrawler implementation capable of crawling it
            Dictionary<CrawlerDataSource, ICrawler> DataSourceToCrawler = CrawlInstantiator.RetrieveCrawlerTranslations();

            //Enumerate existing crawls
            Crawl[] ExistingCrawls = _progress.GetExistingCrawls();
            Crawl ExistingCrawl = ExistingCrawls.Single(c => c.CrawlId == CrawlId);
            ExistingCrawlSpecifier Specifier = new ExistingCrawlSpecifier(ExistingCrawl, ExistingCrawl.Theory.TheoryName, ExistingCrawl.Theory.TheoryComment, ExistingCrawl.TheoryId, ExistingCrawl.Theory.ArticleLevelEigenfactor, ExistingCrawl.Theory.ImpactFactor, ExistingCrawl.Theory.TheoryAttributionRatio, ExistingCrawl.Theory.DataMining, ExistingCrawl.Theory.Clustering, ExistingCrawl.Theory.TheoryDefinitions.ToArray());

            bool Changed = ExistingCrawl.HasChanged;

            Trace.WriteLine(string.Format("Refreshing Crawl", CrawlId), "Informational");
            Dictionary<Source, CanonicalDataSource> CanonicalSources = new Dictionary<Source, CanonicalDataSource>(Specifier.CanonicalDataSources.Length);

            if (Specifier.Crawl.CrawlState == (short)CrawlerState.Started)
            {
                foreach (CanonicalDataSource CanonicalDataSource in Specifier.CanonicalDataSources)
                try
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
                catch {

                }
                Changed = true;
                _progress.SetCrawlerStateChanged(Specifier.Crawl);
                _progress.UpdateCrawlerState(Specifier.Crawl, CrawlerState.CanonicalPaperComplete);
                Trace.WriteLine("Canonical papers retrieved, crawl state committed", "Informational");
            }
            else
            {
                Trace.WriteLine("Crawl already started, retrieving crawl state from database model", "Informational");
                //Find the canonical sources from the database
                foreach (CanonicalDataSource CanonicalDataSource in Specifier.CanonicalDataSources)
                {
                    Source key = _sources.GetSourceByDataSourceSpecificIds(CanonicalDataSource.DataSource, CanonicalDataSource.CanonicalIds);
                    if (key == null)
                    {
                        Trace.WriteLine(CanonicalDataSource, "DEBUG CanonicalDataSource");
                    }
                    else
                    {
                        CanonicalSources[key] = CanonicalDataSource;
                        //CanonicalSources.Add(key, CanonicalDataSource);
                    }
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

                    Source[] CurrentCitationSources = _sources.GetCitingSources(LocalSource).ToArray();
                    string[] CurrentCitations = CurrentCitationSources.Select(r => r.DataSourceSpecificId).ToArray();

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
                            _progress.SetCrawlerStateChanged(Specifier.Crawl);
                            Changed = true;
                        }
                    }

                    _sources.Detach(CurrentCitationSources);
                    CurrentCitationSources = null;
                }

                _progress.CommitQueue();
                _progress.UpdateCrawlerState(Specifier.Crawl, (CrawlerState)(Specifier.Crawl.CrawlState + 1));
                Trace.WriteLine("Canonical paper retrieved, crawl state committed", "Informational");
            }

            if (Specifier.Crawl.CrawlState == (int)CrawlerState.ScheduledCrawlEnqueueingCitationsComplete || Specifier.Crawl.CrawlState == (int)CrawlerState.EnqueueingCitationsComplete)
            {
                if (DequeueCitationsReferences(Specifier.Crawl))
                {
                    _progress.SetCrawlerStateChanged(Specifier.Crawl);
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
                    var Citations = _sources.GetCitingSources(LocalSource).Where(c => c.SourceId > LastReferencedSourceId).OrderBy(c => c.SourceId).ToArray();

                    for (int i = 0; i < Citations.Length; i++)
                    {
                        Source[] CurrentReferenceSources = _sources.GetReferenceSources(Citations[i]).ToArray();
                        string[] CurrentReferences = CurrentReferenceSources.Select(r => r.DataSourceSpecificId).ToArray();

                        Trace.WriteLine(string.Format("Queueing references for paper {0}/{1}", i + 1, Citations.Length));

                        //Get the difference from stored references and current references from data source
                        string[] UpdatedReferences = Crawler.GetReferencesBySourceId(Citations[i].DataSourceSpecificId);
                        string[] NewReferences = UpdatedReferences.Except(CurrentReferences).ToArray();

                        if (NewReferences.Length > 0)
                        {
                            _progress.SetCrawlerStateChanged(Specifier.Crawl);
                            Changed = true;
                        }

                        Trace.WriteLine(string.Format("Found {0} new references", NewReferences.Length));

                        //Queue the retrieved publication IDs for retrieval
                        _progress.QueueReferenceCrawl(Specifier.Crawl.CrawlId, NewReferences, CanonicalSource.DataSource, Citations[i].SourceId, CrawlReferenceDirection.Reference);
                        _progress.CommitQueue();
                        _progress.UpdateCrawlerLastEnumeratedSource(Specifier.Crawl, Citations[i].SourceId);

                        _sources.Detach(Citations[i]);
                        _sources.Detach(CurrentReferenceSources);
                        CurrentReferenceSources = null;
                        Citations[i] = null;
                    }
                }

                _progress.UpdateCrawlerState(Specifier.Crawl, (CrawlerState)(Specifier.Crawl.CrawlState + 1));
                Trace.WriteLine("Reference queueing complete", "Informational");
            }

            if (Specifier.Crawl.CrawlState == (int)CrawlerState.ScheduledCrawlEnqueueingReferencesComplete || Specifier.Crawl.CrawlState == (int)CrawlerState.EnqueueingReferencesComplete)
            {
                if (DequeueCitationsReferences(Specifier.Crawl))
                {
                    _progress.SetCrawlerStateChanged(Specifier.Crawl);
                    Changed = true;
                }
                _progress.UpdateCrawlerState(Specifier.Crawl, CrawlerState.Complete);
                Trace.WriteLine("Dequeueing references complete", "Informational");
            }
            SetModifiedDateIfChanged(Changed, Specifier.TheoryId);
            return Changed;
        }

        /// <summary>
        /// Queue a crawl for a new theory
        /// </summary>
        /// <param name="CrawlSpecifier">The parameters of the theory being crawled</param>
        /// <param name="CrawlIntervalDays">The interval, in days, between refreshes of this crawl. If null, this theory will not be refreshed.</param>
        public Theory StartNewCrawl(NewCrawlSpecifier CrawlSpecifier, int? CrawlIntervalDays = null)
        {
            Theory TheoryToCrawl = _theories.AddTheory(CrawlSpecifier.TheoryName, CrawlSpecifier.TheoryComment, CrawlSpecifier.AEF, CrawlSpecifier.ImpactFactor, CrawlSpecifier.TAR, CrawlSpecifier.DataMining, CrawlSpecifier.Clustering, CrawlSpecifier.CanonicalDataSources);
            PendingCrawlSpecifier pcs = new PendingCrawlSpecifier(TheoryToCrawl.TheoryId, CrawlSpecifier, CrawlIntervalDays, TheoryToCrawl.ArticleLevelEigenfactor, TheoryToCrawl.ImpactFactor, TheoryToCrawl.TheoryAttributionRatio, TheoryToCrawl.DataMining, TheoryToCrawl.Clustering);
            Crawl Crawl = _progress.QueueTheoryCrawl(pcs);
            Trace.WriteLine(string.Format("Queueing crawl using for theory {0}", TheoryToCrawl.TheoryName, "Informational"));
            return TheoryToCrawl;
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
            try
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

                //Mark this queue item as completed
                _progress.CompleteQueueItem(CrawlsToComplete[i], SourceToComplete.SourceId, true);
                Trace.WriteLine(string.Format("Dequeued and retrieved {0}/{1}, Source ID: {2}, Data-Source Specific ID: {3}", i + 1, CrawlsToComplete.Length, SourceToComplete.SourceId, CrawlsToComplete[i].DataSourceSpecificId), "Informational");

                _sources.Detach(SourceToComplete);
                _sources.Detach(CrawlsToComplete[i]);
            } catch {
                
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

        /// <summary>
        /// Sets a theory's modification date if it had previously been changed
        /// </summary>
        /// <param name="Changed">Whether or not the theory has changed</param>
        /// <param name="TheoryId">The TheoryId for the theory having been changed</param>
        public void SetModifiedDateIfChanged(bool Changed, int TheoryId)
        {
            if (Changed)
            {
                _theories.SetLastModifiedDateForTheory(TheoryId, DateTime.Now);
            }
        }
    }
}
