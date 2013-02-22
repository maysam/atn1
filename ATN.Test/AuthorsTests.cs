using System;
using ATN.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ATN.Test
{
    [TestClass]
    public class AuthorsTests : DataUnitTestBase
    {
        public AuthorsTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [TestMethod]
        public void AddAuthor()
        {
            Author AuthorToAdd = new Author();
            AuthorToAdd.FirstName = "Test";
            AuthorToAdd.LastName = "Author";
            AuthorToAdd.FullName = "Test Author";
            AuthorToAdd.Email = "testauthor@example.com";
            AuthorToAdd.DataSourceSpecificId = "999999999999";
            AuthorToAdd.DataSourceId = 1;
            AuthorToAdd.AuthorId = 1;

            Authors a = new Authors(Context);

            //This first call adds the Author
            a.GetAuthorFromDetachedAuthor(AuthorToAdd);

            //This call gets the author out of the repository
            Author AddedAuthor = a.GetAuthorFromDetachedAuthor(AuthorToAdd);
            Assert.AreEqual(AddedAuthor, AuthorToAdd, "Authors are not equal");

            //Cleanup
            Context.DeleteObject(AddedAuthor);
            Context.SaveChanges();
        }

        [TestMethod]
        public void GetAuthor()
        {
            Author AuthorToAdd = new Author();
            AuthorToAdd.FirstName = "Test";
            AuthorToAdd.LastName = "Author";
            AuthorToAdd.FullName = "Test Author";
            AuthorToAdd.Email = "testauthor@example.com";
            AuthorToAdd.DataSourceSpecificId = "999999999999";
            AuthorToAdd.DataSourceId = 1;
            AuthorToAdd.AuthorId = 1;

            Context.Authors.AddObject(AuthorToAdd);
            Context.SaveChanges();

            Authors a = new Authors(Context);

            Author AuthorWithIdOnly = new Author();
            AuthorWithIdOnly.AuthorId = AuthorToAdd.AuthorId;
            AuthorWithIdOnly.DataSourceSpecificId = AuthorToAdd.DataSourceSpecificId;
            AuthorWithIdOnly.DataSourceId = AuthorToAdd.DataSourceId;

            //This call gets the previously added Author
            a.GetAuthorFromDetachedAuthor(AuthorWithIdOnly);

            //This call gets the author out of the repository specifically by id
            Author AddedAuthor = a.GetAuthorFromDetachedAuthor(AuthorWithIdOnly);
            Assert.AreSame(AddedAuthor, AuthorToAdd, "Authors are not equal");

            //Cleanup
            Context.DeleteObject(AddedAuthor);
            Context.SaveChanges();
        }
    }
}
