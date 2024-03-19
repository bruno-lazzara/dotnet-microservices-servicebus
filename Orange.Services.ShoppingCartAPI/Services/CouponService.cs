using Newtonsoft.Json;
using Orange.Models.DTO;
using Orange.Services.ShoppingCartAPI.Services.Interfaces;

namespace Orange.Services.ShoppingCartAPI.Services
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CouponService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<CouponDTO?> GetCouponByCode(string couponCode)
        {
            var client = _httpClientFactory.CreateClient("Coupon");
            var response = await client.GetAsync($"/api/coupon/code/{couponCode}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var coupon = JsonConvert.DeserializeObject<CouponDTO>(content);
                return coupon;
            }

            return null;
        }
    }
}
