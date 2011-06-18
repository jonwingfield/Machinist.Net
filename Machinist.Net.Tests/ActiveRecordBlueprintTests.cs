using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Machinist.Net.Tests.Models.ActiveRecord;
using Machinist.Net.Tests.Drivers;
using System.Data.Entity;

namespace Machinist.Net.Tests
{
    [TestClass]
    public class ActiveRecordBlueprintTests
    {
        private Blueprint GetBlueprints(DbContext context)
        {
            Blueprint bp = new Blueprint();
            bp.Driver = new Machinist.Net.Drivers.EntityFramework.EntityFrameworkActiveRecordDriver(context);
            return bp;
        }

        [TestMethod]
        public void PersistedTest()
        {
            using (BlogDbContext context = new BlogDbContext())
            {
                var blueprints = GetBlueprints(context);

                var user = blueprints.Make<User>();

                context.Database.Connection.Close();

                var users = context.Users;
                Assert.AreEqual(1, users.Count());
                Assert.AreEqual("user1", users.First().Username);
            }
        }

        //[TestMethod]
        //public void WithAssociations()
        //{
        //    var blueprints = GetBlueprints();

        //    var post = blueprints.For<Post>();

        //    Assert.IsNotNull(post.Author);
        //    Assert.AreEqual("user1", post.Author.Username);
        //}

        //[TestMethod]
        //public void AccessOtherAttributes()
        //{
        //    var blueprints = GetBlueprints();

        //    var comment = blueprints.For<Comment>();

        //    Assert.AreEqual("user1@example.com", comment.Email);
        //}

        //[TestMethod]
        //public void Lists()
        //{
        //    var blueprints = GetBlueprints();

        //    var comment = blueprints.For<PostContainer>();

        //    Assert.IsNotNull(comment.Posts);
        //    Assert.AreEqual(4, comment.Posts.Count);
        //}
    }
}
