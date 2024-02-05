using Orange.Models.DTO;

namespace Orange.Web.Services.Interfaces
{
    public interface ICouponService
    {
        Task<CouponDTO?> GetCouponAsync(string couponCode);
        Task<List<CouponDTO>> GetAllAsync();
        Task<CouponDTO?> GetCouponByIdAsync(int id);
        Task<CouponDTO?> CreateCouponAsync(CouponDTO coupon);
        Task<CouponDTO?> UpdateCouponAsync(CouponDTO coupon);
        Task<bool> DeleteCouponAsync(int id);
    }
}
