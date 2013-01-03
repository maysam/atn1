using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crawler.WebCrawler;

namespace Crawler.Persistence
{
    public class Sources : DatabaseInterface
    {
        Authors _authors;
        Journals _journals;
        public Sources()
        {
            _authors = new Authors();
            _journals = new Journals();
        }

        public void AddReference(long SourceId, long CitesSourceId)
        {
            Source CitingSource = Context.Sources.SingleOrDefault(s => s.SourceId == SourceId);
            Source CitedSource = Context.Sources.SingleOrDefault(s => s.SourceId == CitesSourceId);
            CitingSource.Sources.Add(CitedSource);
            Context.SaveChanges();
        }

        public CompleteSource GetCompleteSourceByCanonicalId(CrawlerDataSource DataSource, string CanonicalId)
        {

            SourceRetrievalService SourceRetrival = new SourceRetrievalService();
            Source RetrievedSource = SourceRetrival.GetSourceByDataSourceSpecificId(DataSource, CanonicalId);

            CompleteSource cs = new CompleteSource();
            cs.IsDetached = false;
            cs.Source = RetrievedSource;
            cs.Authors = RetrievedSource.AuthorReferences.Join(Context.Authors, ar => ar.AuthorId, a => a.AuthorId, (ar, a) => a).ToArray();
            cs.Editors = RetrievedSource.EditorReferences.Join(Context.Editors, er => er.EditorId, e => e.EditorId, (er, e) => e).ToArray();
            cs.Journal = RetrievedSource.Journal;

            return cs;
        }
        public Source AddDetachedSource(CompleteSource SourceToAdd)
        {
            if (SourceToAdd.IsDetached)
            {
                List<Persistence.Author> SourceAuthors = new List<Persistence.Author>();
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

                foreach (Author Author in SourceToAdd.Authors)
                {
                    AuthorReference AuthorReference = new AuthorReference();
                    AuthorReference.AuthorId = Author.AuthorId;
                    AuthorReference.SourceId = SourceToAdd.Source.SourceId;
                    Context.AuthorReferences.AddObject(AuthorReference);
                }
                Context.SaveChanges();
                SourceToAdd.IsDetached = false;
            }
            return SourceToAdd.Source;
        }
    }
}
