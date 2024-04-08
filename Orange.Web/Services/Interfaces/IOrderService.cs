using Orange.Models.DTO;

namespace Orange.Web.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderHeaderDTO?> CreateOrderAsync(CartDTO cart);
        Task<StripeRequestDTO?> CreateStripeSessionAsync(StripeRequestDTO stripeRequest);
        Task<OrderHeaderDTO?> ValidateStripeSessionAsync(int orderHeaderId);
        Task<IEnumerable<OrderHeaderDTO>> GetAllOrdersAsync(string? userId);
        Task<OrderHeaderDTO?> GetOrderByIdAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus);
    }
}
