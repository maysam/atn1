using System;
using System.Collections.Generic;
using ATN.Data;
using System.Data.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ATN.Test
{
    [TestClass]
    public class SourcesTests : DataUnitTestBase
    {
        Sources Sources;
        public SourcesTests()
        {
            Sources = new Sources(Context);
        }

        [TestMethod]
        public void VerifyAddDetachedSource()
        {
            CompleteSource cs = new CompleteSource();
            cs.IsDetached = true;

            Author AuthorToAdd = CreateAuthor(false);
            cs.Authors = new Author[] { AuthorToAdd };
            
            Journal JournalToAdd = new Journal();
            JournalToAdd.JournalName = "Test Journal";
            cs.Journal = JournalToAdd;

            Subject SubjectToAdd = CreateSubject(false);
            cs.Subjects = new Subject[] { SubjectToAdd };

            Source SourceToAdd = CreateSource(false);
            cs.Source = SourceToAdd;

            Source AttachedSource = Sources.AddDetachedSource(cs);
            Assert.AreNotEqual(0, AttachedSource.SourceId, "Source was not added");
            Assert.AreEqual(cs.Journal, AttachedSource.Journal, "Journal was not added");
            Assert.AreNotEqual(0, AttachedSource.Journal.JournalId, "Journal was not added");
            Assert.AreEqual(cs.Authors.Length, AttachedSource.AuthorsReferences.Count, "Authors were not added");
            Assert.AreEqual(cs.Subjects.Length, AttachedSource.Subjects.Count, "Subjects were not added");

            CleanupSource(cs);
        }

        [TestMethod]
        public void VerifyGetCompleteSource()
        {
            CompleteSource cs = new CompleteSource();
            cs.IsDetached = true;

            Author AuthorToAdd = CreateAuthor(false);
            cs.Authors = new Author[] { AuthorToAdd };
            cs.Editors = new Editor[0];

            Journal JournalToAdd = new Journal();
            JournalToAdd.JournalName = "Test Journal";
            cs.Journal = JournalToAdd;

            Subject SubjectToAdd = CreateSubject(false);
            cs.Subjects = new Subject[] { SubjectToAdd };

            Source SourceToAdd = CreateSource(false);
            cs.Source = SourceToAdd;
            Sources.AddDetachedSource(cs);

            CompleteSource RetrievedSource = Sources.GetCompleteSourceByDataSourceSpecificId((CrawlerDataSource)cs.Source.DataSourceId, cs.Source.DataSourceSpecificId);

            Assert.AreEqual(cs.Source, RetrievedSource.Source);
            Assert.AreEqual(cs.IsDetached, RetrievedSource.IsDetached, "Attachment not equal");
            Assert.AreEqual(cs.Journal, RetrievedSource.Journal, "Journal not equal");
            Assert.AreEqual(cs.Authors.Length, RetrievedSource.Authors.Length, "Retrieved Authors are not equal");
            for (int i = 0; i < cs.Authors.Length; i++)
            {
                Assert.AreEqual(cs.Authors[i], RetrievedSource.Authors[i], "Retrieved Authors are not equal");
            }
            Assert.AreEqual(cs.Subjects.Length, RetrievedSource.Subjects.Length, "Retrieved Subjects are not equal");
            for (int i = 0; i < cs.Subjects.Length; i++)
            {
                Assert.AreEqual(cs.Subjects[i], RetrievedSource.Subjects[i], "Retrieved Subjects are not equal");
            }
            CleanupSource(cs);
        }

        [TestMethod]
        public void VerifyGetSourceByIds()
        {
            Source AddedSource = CreateSource(true);
            Source RetrievedSource = Sources.GetSourceByDataSourceSpecificIds((CrawlerDataSource)AddedSource.DataSourceId, new string[] { AddedSource.DataSourceSpecificId });
            Assert.AreEqual(AddedSource, RetrievedSource, "Sources are not equal");
            DeleteSource(AddedSource.SourceId);
        }

        [TestMethod]
        public void VerifyGetSourceById()
        {
            Source AddedSource = CreateSource(true);
            Source RetrievedSource = Sources.GetSourceByDataSourceSpecificId((CrawlerDataSource)AddedSource.DataSourceId, AddedSource.DataSourceSpecificId );
            Assert.AreEqual(AddedSource, RetrievedSource, "Sources are not equal");
            DeleteSource(AddedSource.SourceId);
        }

        [TestMethod]
        public void VerifyAddCitation()
        {
            Source CitedSource = CreateSource(true);
            Source CitingSource = CreateSource(true);

            Sources.AddCitation(CitingSource.SourceId, CitedSource.SourceId);

            Source Reference = CitingSource.References.SingleOrDefault();
            Assert.AreEqual(CitedSource, Reference, "Citation not added");

            CitingSource.References.Remove(CitedSource);
            Context.SaveChanges();
            DeleteSource(CitedSource.SourceId);
            DeleteSource(CitingSource.SourceId);
        }

        private void CleanupSource(CompleteSource AttachedCompleteSource)
        {
            foreach (AuthorsReference ar in AttachedCompleteSource.Source.AuthorsReferences.ToArray())
            {
                Author AuthorToDelete = ar.Author;
                DeleteAuthorsReference(ar);
                //DeleteAuthor(AuthorToDelete);
            }
            foreach (Subject s in AttachedCompleteSource.Source.Subjects.ToArray())
            {
                DeleteSubject(s);
            }
            DeleteSource(AttachedCompleteSource.Source.SourceId);
            //DeleteJournal(cs.Journal);
        }
    }
}
