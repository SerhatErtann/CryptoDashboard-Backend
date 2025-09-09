using CryptoDashboard.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CryptoDashboard.Data
{
    public class CryptoDbContext : DbContext
    {
        public CryptoDbContext(DbContextOptions<CryptoDbContext> options)
            : base(options)
        { }

        public DbSet<CryptoPrice> CryptoPrice { get; set; }
    }
}
