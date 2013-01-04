using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler.Persistence
{
    /// <summary>
    /// A service for retrieving and storing authors
    /// </summary>
    public class Authors : DatabaseInterface
    {
        public Authors()
        {

        }

        /// <summary>
        /// Determines if the passed Author exists in the persistence-model, and adds it if it does not
        /// </summary>
        /// <param name="DetachedAuthor">The Author with which to check existence, and add if it does not exist</param>
        /// <returns>A persistence-model attached copy of the passed Author</returns>
        public Author GetAuthorFromDetachedAuthor(Author DetachedAuthor)
        {
            Author PersistentAuthor = Context.Authors.Where(a => a.DataSourceId == DetachedAuthor.DataSourceId && a.DataSourceSpecificId == DetachedAuthor.DataSourceSpecificId).SingleOrDefault();
            if (PersistentAuthor == null)
            {
                PersistentAuthor = DetachedAuthor;
                Context.Authors.AddObject(DetachedAuthor);
                Context.SaveChanges();
            }
            return PersistentAuthor;
        }
    }
}
