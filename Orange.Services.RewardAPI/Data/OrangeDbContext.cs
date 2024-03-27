using Microsoft.EntityFrameworkCore;
using Orange.Services.RewardAPI.Models.Entity;

namespace Orange.Services.RewardAPI.Data
{
    public class OrangeDbContext : DbContext
    {
        public OrangeDbContext(DbContextOptions<OrangeDbContext> options) : base(options)
        {
        }

        public DbSet<Rewards> Rewards { get; set; }
    }
}
