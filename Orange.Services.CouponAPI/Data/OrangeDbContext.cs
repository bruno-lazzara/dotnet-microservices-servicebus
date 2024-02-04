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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Coupon>().HasData(new Coupon
            {
                CouponId = 1,
                CouponCode = "10off",
                DiscountAmount = 10,
                MinAmount = 30,
            }, new Coupon
            {
                CouponId = 2,
                CouponCode = "15off",
                DiscountAmount = 15,
                MinAmount = 40,
            });
        }
    }
}
