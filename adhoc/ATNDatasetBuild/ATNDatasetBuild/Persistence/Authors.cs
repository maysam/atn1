using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler.Persistence
{
    public class Authors : DatabaseInterface
    {
        public Authors()
        {

        }

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
