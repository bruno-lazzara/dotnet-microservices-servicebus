using Orange.Models.DTO;

namespace Orange.Web.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderHeaderDTO?> CreateOrderAsync(CartDTO cart);
        Task<StripeRequestDTO?> CreateStripeSessionAsync(StripeRequestDTO stripeRequest);
    }
}
