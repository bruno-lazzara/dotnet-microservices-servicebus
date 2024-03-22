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
    }
}
