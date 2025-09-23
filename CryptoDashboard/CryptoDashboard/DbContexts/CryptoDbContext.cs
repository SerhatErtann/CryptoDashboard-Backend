using CryptoDashboard.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CryptoDashboard.DbContexts
{
    public class CryptoDbContext : DbContext
    {
        public CryptoDbContext(DbContextOptions<CryptoDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CryptoPrice>()
                .HasKey(c => c.SNo);   
        }

        public DbSet<CryptoPrice> CryptoPrice { get; set; }
    }
}
