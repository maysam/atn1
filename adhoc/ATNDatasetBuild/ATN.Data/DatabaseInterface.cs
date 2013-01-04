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
        public DatabaseInterface()
        {
            _context = new ATNEntities();
        }

        /// <summary>
        /// The persistence-model object used to communicate with database model
        /// </summary>
        public ATNEntities Context
        {
            get
            {
                return _context;
            }
        }
    }
}
