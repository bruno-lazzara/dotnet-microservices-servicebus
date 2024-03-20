using Orange.Models.DTO;

namespace Orange.Web.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartDTO?> GetCartByUserIdAsync(string userId);
        Task<CartDTO?> UpsertCartAsync(CartDTO cart);
        Task<bool> RemoveFromCartAsync(int cartDetailsId);
        Task<bool> ApplyCouponAsync(CartDTO cart);
        Task<bool> EmailCartAsync(CartDTO cart);
    }
}
