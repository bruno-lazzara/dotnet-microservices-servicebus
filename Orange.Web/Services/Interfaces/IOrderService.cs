using Orange.Models.DTO;

namespace Orange.Web.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderHeaderDTO?> CreateOrderAsync(CartDTO cart);
        Task<StripeRequestDTO?> CreateStripeSessionAsync(StripeRequestDTO stripeRequest);
        Task<OrderHeaderDTO?> ValidateStripeSessionAsync(int orderHeaderId);
        Task<IEnumerable<OrderHeaderDTO>> GetAllOrders(string? userId);
        Task<OrderHeaderDTO?> GetOrder(int orderId);
        Task<bool> UpdateOrderStatus(int orderId, string newStatus);
    }
}
