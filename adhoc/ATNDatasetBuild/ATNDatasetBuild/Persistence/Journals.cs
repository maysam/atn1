using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler.Persistence
{
    public class Journals : DatabaseInterface
    {
        public Journals()
        {

        }

        public Journal GetJournalFromDetachedJournal(Journal DetachedJournal)
        {
            Journal PersistentJournal = Context.Journals.Where(j => j.JournalName.ToLower().Trim() == DetachedJournal.JournalName.ToLower().Trim()).SingleOrDefault();
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
