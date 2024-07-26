using Microsoft.EntityFrameworkCore;
using Membina_Currency.Models.Domain;

namespace Membina_Currency.Data
{
    public class MembinaCurrencyDbContext : DbContext
    {
        public MembinaCurrencyDbContext(DbContextOptions options) : base(options)
        {
            
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Currency> Currencies { get; set; }
    }
}
