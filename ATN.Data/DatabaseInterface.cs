using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATN.Data
{
    /// <summary>
    /// A representation of the data source for a specific crawl
    /// </summary>
    public enum CrawlerDataSource { MicrosoftAcademicSearch = 1 };

    /// <summary>
    /// A light-weight service for providing persistence-model interaction
    /// </summary>
    public abstract class DatabaseInterface
    {
        private ATNEntities _context;
        public DatabaseInterface(ATNEntities Entities = null)
        {
            if (Entities == null)
            {
                _context = new ATNEntities();
            }
            else
            {
                _context = Entities;
            }
        }

        public void Cleanup()
        {
            _context.Dispose();
        }

        /// <summary>
        /// The persistence-model object used to communicate with database model
        /// </summary>
        protected ATNEntities Context
        {
            get
            {
                return _context;
            }
        }
    }
}
