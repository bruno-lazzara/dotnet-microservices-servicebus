using Orange.Models.DTO;

namespace Orange.Services.ShoppingCartAPI.Services.Interfaces
{
    public interface ICouponService
    {
        Task<CouponDTO?> GetCouponByCode(string couponCode);
    }
}
