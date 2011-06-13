using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Machinist.Net.Tests.Models.ActiveRecord;

namespace Machinist.Net.Tests.Drivers
{
    public class BlogDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
    }
}
