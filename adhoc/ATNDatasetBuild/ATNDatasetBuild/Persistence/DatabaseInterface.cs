using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler.Persistence
{
    public enum CrawlerDataSource { MicrosoftAcademicSearch = 1 };
    public abstract class DatabaseInterface
    {
        private ATNEntities _context;
        public DatabaseInterface()
        {
            _context = new ATNEntities();
        }
        public ATNEntities Context
        {
            get
            {
                return _context;
            }
        }
    }
}
