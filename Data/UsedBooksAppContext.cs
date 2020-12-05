using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UsedBooksApp.Models;

namespace UsedBooksApp.Data
{
    public class UsedBooksAppContext : DbContext
    {
        public UsedBooksAppContext (DbContextOptions<UsedBooksAppContext> options)
            : base(options)
        {
        }

        public DbSet<UsedBooksApp.Models.Book> Book { get; set; }
    }
}
