using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Machinist.Net.Tests.Models.ActiveRecord;
using Machinist.Net;
using Faker;

namespace Machinist.Net.Tests
{
    public class Blueprints : BlueprintDefinition
    {
        public Blueprints()
        {
            Blueprint<User>(user =>
            {
                user.Username = "user#{sn}";
            });
            Blueprint<User>("Admin", user =>
            {
                user.Username = "Admin"; 
            });
            Blueprint<Post>(post =>
            {
                post.Body = "A Body";
                post.Title = "A title";
            }).WithAssociations();
            Blueprint<Post>("NoAssociations");
            Blueprint<Comment>(comment =>
            {
                comment.Email = "#{object.Post.Author.Username}@example.com";
                comment.Body = "A comment";
            }).WithAssociations();
            Blueprint<PostContainer>(container =>
            {
                container.Posts = listOf<Comment>(4);
                container.Id = 5;
            }).WithAssociations();
        }
    }

    [TestClass]
    public class BaseBlueprintTests
    {
        Blueprint blueprints;

        [TestInitialize]
        public void TestInitialize()
        {
            blueprints = new Blueprint();
        }

        [TestMethod]
        public void SerialNumbers()
        {
            Assert.AreEqual("user1", blueprints.Make<User>().Username);
            Assert.AreEqual("user2", blueprints.Make<User>().Username);
        }

        [TestMethod]
        public void Overrides()
        {
            Assert.AreEqual("user20", blueprints.Make<User>(x => x.Username = "user20").Username);
        }

        [TestMethod]
        public void NamedBlueprints()
        {
            Assert.AreEqual("Admin", blueprints.Make<User>("Admin").Username);
        }

        [TestMethod]
        public void IdAutomaticallyGenerated()
        {
            Assert.AreEqual(1, blueprints.Make<User>().UserId);
            Assert.AreEqual(2, blueprints.Make<User>().UserId);
        }

        [TestMethod]
        public void IdNotGeneratedIfAlreadySet()
        {
            Assert.AreEqual(5, blueprints.Make<PostContainer>().Id);
        }

        [TestMethod]
        public void IdsUniquePerObject()
        {
            Assert.AreEqual(1, blueprints.Make<User>().UserId);
            Assert.AreEqual(1, blueprints.Make<Post>().PostId);
            Assert.AreEqual(3, blueprints.Make<User>().UserId); // this is 3 because Post.User.UserId == 2
        }

        [TestMethod]
        public void GuidAutomaticallyGenerated()
        {
            Assert.AreNotEqual(Guid.Empty, blueprints.Make<Comment>().CommentGuid);
        }

        [TestMethod]
        public void AssociationsNotGeneratedByDefault()
        {
            Assert.IsNull(blueprints.Make<Post>("NoAssociations").Author);
        }

        [TestMethod]
        public void WithAssociations()
        {
            var post = blueprints.Make<Post>();

            Assert.IsNotNull(post.Author);
            Assert.AreEqual("user1", post.Author.Username);
        }

        [TestMethod]
        public void AccessOtherAttributes()
        {
            var comment = blueprints.Make<Comment>();

            Assert.AreEqual("user1@example.com", comment.Email);
        }

        [TestMethod]
        public void Lists()
        {
            var comment = blueprints.Make<PostContainer>();

            Assert.IsNotNull(comment.Posts);
            Assert.AreEqual(4, comment.Posts.Count);
        }


    }
}
