using learnpoint_test_consoleApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace learnpoint_test_consoleApp.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Resource> Resources { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLOCALDB;Initial Catalog=testDB;MultipleActiveResultSets=False;Connection Timeout=30;Trusted_Connection=True");
        }

        // Server=tcp:testservereg.database.windows.net,1433;Initial Catalog=testDB;Persist Security Info=False;User ID=eg;Password=1234567Aa;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Resource>().OwnsOne(p => p.SourceId);
        //    modelBuilder.Entity<Resource>().OwnsOne(p => p.TargetId);
        //}

    }
}