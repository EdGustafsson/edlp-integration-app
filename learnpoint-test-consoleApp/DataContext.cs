using Microsoft.EntityFrameworkCore;
using learnpoint_test_consoleApp.Models;

namespace learnpoint_test_consoleApp
{
    public class DataContext : DbContext
    {
        public DbSet<Resource> Resources { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=EIADB.db");
        }
    }
}