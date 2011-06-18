using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Machinist.Net.Tests.Models.ActiveRecord
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
    }

    public class Post
    {
        public User Author { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public long PostId { get; set; }
    }

    public class Comment
    {
        public Post Post { get; set; }
        public string Email { get; set; }
        public string Body { get; set; }
        public Guid CommentGuid { get; set; }
    }

    public class PostContainer
    {
        public Post Post { get; set; }
        public List<Comment> Posts { get; set; }
        public int Id { get; set; }
    }
}
