using Microsoft.EntityFrameworkCore;
using Orange.Services.CouponAPI.Models.Entity;

namespace Orange.Services.CouponAPI.Data
{
    public class OrangeDbContext : DbContext
    {
        public OrangeDbContext(DbContextOptions<OrangeDbContext> options) : base(options)
        {
        }

        public DbSet<Coupon> Coupons { get; set; }
    }
}
