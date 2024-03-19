using Microsoft.EntityFrameworkCore;
using Orange.Services.ShoppingCartAPI.Models.Entity;

namespace Orange.Services.ShoppingCartAPI.Data
{
    public class OrangeDbContext : DbContext
    {
        public OrangeDbContext(DbContextOptions<OrangeDbContext> options) : base(options)
        {
        }

        public DbSet<CartHeader> CartHeaders { get; set; }
        public DbSet<CartDetails> CartDetails { get; set; }
    }
}
