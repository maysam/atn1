using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATN.Data
{
    /// <summary>
    /// A service for interacting with Journal objects from the persistence model
    /// </summary>
    public class Journals : DatabaseInterface
    {
        public Journals(ATNEntities Entities = null)
            : base(Entities)
        {

        }

        /// <summary>
        /// Determines if the passed Journal exists in the persistence-model, and adds it if it does not
        /// </summary>
        /// <param name="DetachedJournal">The Journal for which to check existence</param>
        /// <returns>A persistence-model attached copy of the passed Journal</returns>
        public Journal GetJournalFromDetachedJournal(Journal DetachedJournal)
        {
            Journal PersistentJournal = Context.Journals.FirstOrDefault(j => j.JournalName.ToLower().Trim() == DetachedJournal.JournalName.ToLower().Trim());
            if (PersistentJournal == null)
            {
                PersistentJournal = DetachedJournal;
                Context.Journals.AddObject(PersistentJournal);
                Context.SaveChanges();
            }
            return PersistentJournal;
        }
    }
}
