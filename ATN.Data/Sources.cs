﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Diagnostics;

namespace ATN.Data
{
    /// <summary>
    /// A service for interacting with Source objects from the persistence-model
    /// </summary>
    public class Sources : DatabaseInterface
    {
        Authors _authors;
        Journals _journals;
        Subjects _subjects;
        public Sources(ATNEntities Entities = null) : base(Entities)
        {
            _authors = new Authors(Entities);
            _journals = new Journals(Entities);
            _subjects = new Subjects(Entities);
        }

        /// <summary>
        /// Retrieves a list of all existing sources
        /// </summary>
        /// <returns>A list of all existing sources</returns>
        public Source[] GetSources()
        {
            return Context.Sources.ToArray();
        }

        public Source GetSourceById(long SourceId)
        {
            return Context.Sources.Single(s => s.SourceId == SourceId);
        }

        public EntityCollection<Source> GetCitingSources(Source SourceToRetrieveCitations)
        {
            return Context.Sources.Single(s => s.SourceId == SourceToRetrieveCitations.SourceId).CitingSources;
        }

        public EntityCollection<Source> GetReferenceSources(Source SourceToRetrieveReferences)
        {
            return Context.Sources.Single(s => s.SourceId == SourceToRetrieveReferences.SourceId).References;
        }

        /// <summary>
        /// Adds a citation from one Source to another
        /// </summary>
        /// <param name="SourceId">The citing Source</param>
        /// <param name="CitesSourceId">The cited Source</param>
        public void AddCitation(long SourceId, long CitesSourceId)
        {
            Source CitingSource = Context.Sources.SingleOrDefault(s => s.SourceId == SourceId);
            Source CitedSource = Context.Sources.SingleOrDefault(s => s.SourceId == CitesSourceId);
            try
            {
                CitedSource.CitingSources.Add(CitingSource);
                Context.SaveChanges();
            }
            catch
            {
                CitedSource.CitingSources.Remove(CitingSource);
                Trace.WriteLine(string.Format("Source ID {0} already cites Source ID {1}", SourceId, CitesSourceId), "Informational");
            }
        }

        /// <summary>
        /// Retrieves the unique Source corresponding to the given data source and passed data-source specific identifier
        /// </summary>
        /// <param name="DataSource">The data-source from which to retrieve</param>
        /// <param name="DataSourceSpecificId">The data-source specific identifier for which to retrieve</param>
        /// <returns>A unique Source corresponding to the given data source and data-source specific identifier</returns>
        public Source GetSourceByDataSourceSpecificId(CrawlerDataSource DataSource, string DataSourceSpecificId)
        {
            return Context.Sources.Where(s => s.DataSourceId == (int)DataSource && s.DataSourceSpecificId == DataSourceSpecificId).SingleOrDefault();
        }

        public Source GetSourceByDataSourceSpecificIds(CrawlerDataSource DataSource, string[] DataSourceSpecificIds)
        {
            return Context.Sources.Where(s => s.DataSourceId == (int)DataSource && DataSourceSpecificIds.Contains(s.DataSourceSpecificId)).FirstOrDefault();
            Source SourceToReturn = null;

            //Find the canonical source from the database, stopping once one is found
            for (int i = 0; i < DataSourceSpecificIds.Length && SourceToReturn == null; i++)
            {
                SourceToReturn = GetSourceByDataSourceSpecificId(DataSource, DataSourceSpecificIds[i]);
            }

            return SourceToReturn;
        }

        public void UpdateExternalURL(Source SourceToUpdate, string ExternalURL)
        {
            SourceToUpdate.ExternalURL = ExternalURL;
            Context.SaveChanges();
        }

        /// <summary>
        /// Retrieve a full representation of a Source
        /// </summary>
        /// <param name="DataSource">The data-source from which to retrieve the Source</param>
        /// <param name="DataSourceSpecificId">The data-source specific identifier of the Source to be retrieved</param>
        /// <returns>A complete representation of the desired Source</returns>
        public CompleteSource GetCompleteSourceByDataSourceSpecificId(CrawlerDataSource DataSource, string DataSourceSpecificId)
        {
            Source RetrievedSource = GetSourceByDataSourceSpecificId(DataSource, DataSourceSpecificId);

            CompleteSource cs = new CompleteSource();
            cs.IsDetached = false;
            cs.Source = RetrievedSource;
            cs.Authors = RetrievedSource.AuthorsReferences.Join(Context.Authors, ar => ar.AuthorId, a => a.AuthorId, (ar, a) => a).ToArray();
            cs.Editors = RetrievedSource.EditorsReferences.Join(Context.Editors, er => er.EditorId, e => e.EditorId, (er, e) => e).ToArray();
            cs.Journal = RetrievedSource.Journal;
            cs.Subjects = RetrievedSource.Subjects.ToArray();

            return cs;
        }

        /// <summary>
        /// Retrieve a full representation of a Source
        /// </summary>
        /// <param name="sourceId">The source ID with which to retrieve the Source</param>
        /// <returns>A complete representation of the desired Source</returns>
        public CompleteSource GetCompleteSourceBySourceId(long sourceId)
        {
            Source RetrievedSource = Context.Sources.Single(s => s.SourceId == sourceId);

            CompleteSource cs = new CompleteSource();
            cs.IsDetached = false;
            cs.Source = RetrievedSource;
            cs.Authors = RetrievedSource.AuthorsReferences.Join(Context.Authors, ar => ar.AuthorId, a => a.AuthorId, (ar, a) => a).ToArray();
            cs.Editors = RetrievedSource.EditorsReferences.Join(Context.Editors, er => er.EditorId, e => e.EditorId, (er, e) => e).ToArray();
            cs.Journal = RetrievedSource.Journal;
            cs.Subjects = RetrievedSource.Subjects.ToArray();

            return cs;
        }

        /*public ExtendedSource GetExtendedSourceBySourceId(int theoryId, long sourceId)
        {
            CompleteSource RetrievedSource = GetCompleteSourceBySourceId(sourceId);
            ExtendedSource es = new ExtendedSource(RetrievedSource);

            AnalysisInterface ai = new AnalysisInterface(Context);
            CompleteTheoryMembership ctm = ai.GetTheoryMembershipContributionsForSource(theoryId, sourceId);

            es.aefScore = ctm.TheoryMembership.ArticleLevelEigenFactor;
            es.isContributing = ctm.TheoryMembershipSignificance.RAMarkedContributing;
            es.metaAnalysis = ctm.TheoryMembershipSignificance.IsMetaAnalysis;
            es.numContributing = ctm.NumberContributing;
            es.depth = ctm.TheoryMembership.Depth;
            return es;
        }*/

        /// <summary>
        /// Adds a detached CompleteSource object
        /// </summary>
        /// <param name="SourceToAdd">A complete representation of the Source to add</param>
        /// <returns>An attached Source object corresponding to the complete Source representation</returns>
        public Source AddDetachedSource(CompleteSource SourceToAdd)
        {
            if (SourceToAdd.IsDetached)
            {
                List<Author> SourceAuthors = new List<Author>();
                foreach (var Author in SourceToAdd.Authors)
                {
                    SourceAuthors.Add(_authors.GetAuthorFromDetachedAuthor(Author));
                }
                SourceToAdd.Authors = SourceAuthors.ToArray();
                if (SourceToAdd.Journal != null)
                {
                    SourceToAdd.Journal = _journals.GetJournalFromDetachedJournal(SourceToAdd.Journal);
                }

                if (SourceToAdd.Journal != null)
                {
                    SourceToAdd.Source.JournalId = SourceToAdd.Journal.JournalId;
                }
                Context.Sources.AddObject(SourceToAdd.Source);
                Context.SaveChanges();

                if(SourceToAdd.Authors != null)
                foreach (Author Author in SourceToAdd.Authors)
                {
                    AuthorsReference AuthorReference = new AuthorsReference();
                    AuthorReference.AuthorId = Author.AuthorId;
                    AuthorReference.SourceId = SourceToAdd.Source.SourceId;
                    Context.AuthorsReferences.AddObject(AuthorReference);
                }
                
                if(SourceToAdd.Subjects != null)
                foreach (Subject Subject in SourceToAdd.Subjects)
                {
                    Subject AlwaysBoundSubject = _subjects.GetOrAddSubject(Subject);
                    _subjects.AddSubjectToSource(SourceToAdd.Source.SourceId, AlwaysBoundSubject.SubjectId);
                }

                Context.SaveChanges();
                SourceToAdd.IsDetached = false;
            }
            return SourceToAdd.Source;
        }
    }
}