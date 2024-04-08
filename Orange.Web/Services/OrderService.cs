using Newtonsoft.Json;
using Orange.Models.DTO;
using Orange.Web.Services.Interfaces;
using Orange.Web.Utils;

namespace Orange.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;
        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<OrderHeaderDTO?> CreateOrderAsync(CartDTO cart)
        {
            OrderHeaderDTO? orderResponse = null;
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Post,
                    Url = Routes.OrderAPI + $"/api/order/CreateOrder",
                    Data = cart
                });

                if (response != null && response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    orderResponse = JsonConvert.DeserializeObject<OrderHeaderDTO>(content);
                }
            }
            catch (Exception ex)
            {

            }
            return orderResponse;
        }

        public async Task<StripeRequestDTO?> CreateStripeSessionAsync(StripeRequestDTO stripeRequest)
        {
            StripeRequestDTO? stripeResponse = null;
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Post,
                    Url = Routes.OrderAPI + $"/api/order/CreateStripeSession",
                    Data = stripeRequest
                });

                if (response != null && response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    stripeResponse = JsonConvert.DeserializeObject<StripeRequestDTO>(content);
                }
            }
            catch (Exception ex)
            {

            }

            return stripeResponse;
        }

        public async Task<IEnumerable<OrderHeaderDTO>> GetAllOrders(string? userId)
        {
            IEnumerable<OrderHeaderDTO>? orderResponse = [];
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Get,
                    Url = Routes.OrderAPI + $"/api/order/GetOrders?userId={userId}"
                });

                if (response != null && response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    orderResponse = JsonConvert.DeserializeObject<IEnumerable<OrderHeaderDTO>>(content);
                }
            }
            catch (Exception ex)
            {

            }
            return orderResponse ?? [];
        }

        public async Task<OrderHeaderDTO?> GetOrder(int orderId)
        {
            OrderHeaderDTO? orderResponse = null;
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Get,
                    Url = Routes.OrderAPI + $"/api/order/GetOrder/{orderId}"
                });

                if (response != null && response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    orderResponse = JsonConvert.DeserializeObject<OrderHeaderDTO>(content);
                }
            }
            catch (Exception ex)
            {

            }
            return orderResponse;
        }

        public async Task<bool> UpdateOrderStatus(int orderId, string newStatus)
        {
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Post,
                    Url = Routes.OrderAPI + $"/api/order/UpdateOrderStatus/{orderId}",
                    Data = newStatus
                });

                return response != null && response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<OrderHeaderDTO?> ValidateStripeSessionAsync(int orderHeaderId)
        {
            OrderHeaderDTO? orderResponse = null;
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Post,
                    Url = Routes.OrderAPI + $"/api/order/ValidateStripeSession",
                    Data = orderHeaderId
                });

                if (response != null && response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    orderResponse = JsonConvert.DeserializeObject<OrderHeaderDTO>(content);
                }
            }
            catch (Exception ex)
            {
                
            }
            return orderResponse;
        }
    }
}
