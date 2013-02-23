using System;
using ATN.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ATN.Test
{
    [TestClass]
    public class JournalsTests : DataUnitTestBase
    {
        Journals Journals;
        public JournalsTests()
        {
            Journals = new Journals(Context);
        }

        [TestMethod]
        public void VerifyAddJournal()
        {
            Journal JournalToAdd = new Journal();
            JournalToAdd.JournalName = "Test Journal";
            Journal AddedJournal = Journals.GetJournalFromDetachedJournal(JournalToAdd);
            DeleteJournal(AddedJournal);

            Assert.AreNotEqual(0, AddedJournal.JournalId, "Journal was not added");
        }

        [TestMethod]
        public void VerifyDuplicateJournalIsPreviousJournal()
        {
            Journal JournalToAdd = new Journal();
            JournalToAdd.JournalName = "Test Journal";
            Journal AddedJournal = Journals.GetJournalFromDetachedJournal(JournalToAdd);

            Journal DuplicateJournal = new Journal();
            DuplicateJournal.JournalName = JournalToAdd.JournalName;
            Journal DuplicateAddedJournal = Journals.GetJournalFromDetachedJournal(DuplicateJournal);
            

            DeleteJournal(AddedJournal);

            Assert.AreEqual(AddedJournal.JournalId, DuplicateAddedJournal.JournalId, "Duplicate Journal should have been replaced with existing Journal");
        }
    }
}
