using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Machinist.Net.Tests.Models.ActiveRecord;
using Machinist.Net;

namespace Machinist.Net.Tests
{
    public class Blueprints : BlueprintBase
    {
        protected override void Init()
        {
            Blueprint<User>(user =>
            {
                user.Username = "user" + serial_number;
            });
            Blueprint<Post>(post =>
            {
                post.Body = "A Body";
                post.Title = "A title";
            });
            Blueprint<Comment>(comment =>
            {
                comment.Email = comment.Post.Author.Username + "@example.com";
                comment.Body = "A comment";
            });
            Blueprint<PostContainer>(container =>
            {
                container.Posts = listOf<Comment>(4);
            });
        }
    }

    [TestClass]
    public class BaseBlueprintTests
    {
        private Blueprints GetBlueprints()
        {
            return new Blueprints();
        }

        [TestMethod]
        public void SerialNumbers()
        {
            var blueprints = GetBlueprints();

            Assert.AreEqual("user1", blueprints.For<User>().Username);
            Assert.AreEqual("user2", blueprints.For<User>().Username);
        }

        [TestMethod]
        public void WithAssociations()
        {
            var blueprints = GetBlueprints();

            var post = blueprints.For<Post>();

            Assert.IsNotNull(post.Author);
            Assert.AreEqual("user1", post.Author.Username);
        }

        [TestMethod]
        public void AccessOtherAttributes()
        {
            var blueprints = GetBlueprints();

            var comment = blueprints.For<Comment>();

            Assert.AreEqual("user1@example.com", comment.Email);
        }

        [TestMethod]
        public void Lists()
        {
            var blueprints = GetBlueprints();

            var comment = blueprints.For<PostContainer>();

            Assert.IsNotNull(comment.Posts);
            Assert.AreEqual(4, comment.Posts.Count);
        }

    }
}
