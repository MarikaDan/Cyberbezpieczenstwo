using LAB1.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LAB1.Data
{
    public class UsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }
    }
}
