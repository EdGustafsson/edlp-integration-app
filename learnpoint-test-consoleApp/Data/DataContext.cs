﻿using Microsoft.EntityFrameworkCore;
using learnpoint_test_consoleApp.Entities;

namespace learnpoint_test_consoleApp.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Resource> Resources { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=EIADB.db");
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Resource>().OwnsOne(p => p.SourceId);
        //    modelBuilder.Entity<Resource>().OwnsOne(p => p.TargetId);
        //}    

    }
}