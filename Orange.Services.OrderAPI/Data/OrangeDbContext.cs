using Microsoft.EntityFrameworkCore;
using Orange.Services.OrderAPI.Models.Entity;

namespace Orange.Services.OrderAPI.Data
{
    public class OrangeDbContext : DbContext
    {
        public OrangeDbContext(DbContextOptions<OrangeDbContext> options) : base(options)
        {
        }

        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
    }
}
