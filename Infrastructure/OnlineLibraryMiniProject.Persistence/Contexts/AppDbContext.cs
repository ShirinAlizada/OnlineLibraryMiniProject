using Microsoft.EntityFrameworkCore;
using OnlineLibraryMiniProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace OnlineLibraryMiniProject.Persistence.Contexts
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configure the database connection string here
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("server=(localdb)\\MSSQLLocalDB;database=OnlineLibraryProject;trusted_connection=true;integrated security=true;trustservercertificate=true");
        }
       
            //public DbSet<Book> Books { get; set; }
        //public DbSet<Author> Authors { get; set; }
        //public DbSet<ReservedItem> ReservedItems { get; set; }
    }
}
