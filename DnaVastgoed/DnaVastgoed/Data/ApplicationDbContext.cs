using DnaVastgoed.Models;
using Microsoft.EntityFrameworkCore;

namespace DnaVastgoed.Data {

    public class ApplicationDbContext : DbContext {

        public DbSet<DnaProperty> Properties { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=Database.db");
    }
}
