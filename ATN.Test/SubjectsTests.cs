using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using ATN.Data;

namespace ATN.Test
{
    [TestClass]
    public class SubjectsTests : DataUnitTestBase
    {
        Subjects Subjects;
        public SubjectsTests()
        {
            Subjects = new Subjects(Context);
        }

        [TestMethod]
        public void VerifyGetSubjectById()
        {
            Subject AddedSubject = CreateSubject(true);
            Subject RetrievedSubject = Subjects.GetSubjectById(AddedSubject.SubjectId);
            Assert.AreEqual(AddedSubject, RetrievedSubject, "Retrieved Subject not equal");

            DeleteSubject(AddedSubject);
        }

        [TestMethod]
        public void VerifyGetSubjectByDataSourceId()
        {
            Subject AddedSubject = CreateSubject(true);
            Subject RetrievedSubject = Subjects.GetSubjectByDataSourceId(AddedSubject.DataSourceSpecificId, (CrawlerDataSource)AddedSubject.DataSourceId);
            Assert.AreEqual(AddedSubject, RetrievedSubject, "Retrieved Subject not equal");

            DeleteSubject(AddedSubject);
        }

        [TestMethod]
        public void VerifyAddSubjectToSource()
        {
            Subject AddedSubject = CreateSubject(true);
            Source AddedSource = CreateSource(true);

            Subjects.AddSubjectToSource(AddedSource.SourceId, AddedSubject.SubjectId);

            Subject RetrievedSubject = AddedSource.Subjects.SingleOrDefault();
            Assert.AreEqual(AddedSubject, RetrievedSubject, "Subject was not added to Source");

            AddedSource.Subjects.Remove(RetrievedSubject);
            Context.SaveChanges();
            DeleteSubject(AddedSubject);
            DeleteSource(AddedSource.SourceId);
        }

        [TestMethod]
        public void VerifyGetOrAddSubjectAddsSubject()
        {
            Subject AddedSubject = CreateSubject(false);
            Subject BoundSubject = Subjects.GetOrAddSubject(AddedSubject);
            Assert.AreEqual(AddedSubject, BoundSubject, "Subject was not added");
            Assert.AreNotEqual(0, BoundSubject.SubjectId, "Subject was not added");

            DeleteSubject(AddedSubject);
        }

        [TestMethod]
        public void VerifyGetOrAddSubjectRetrievesSubject()
        {
            Subject AddedSubject = CreateSubject(true);
            Subject RetrievedSubject = Subjects.GetOrAddSubject(AddedSubject);
            Assert.AreSame(AddedSubject, RetrievedSubject, "Source was added again");

            DeleteSubject(AddedSubject);
        }
    }
}
