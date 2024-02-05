using Newtonsoft.Json;
using Orange.Models.DTO;
using Orange.Web.Services.Interfaces;
using Orange.Web.Utils;

namespace Orange.Web.Services
{
    public class CouponService : ICouponService
    {
        private readonly IBaseService _baseService;
        public CouponService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<CouponDTO?> CreateCouponAsync(CouponDTO coupon)
        {
            CouponDTO? couponResponse = null;
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Post,
                    Url = Routes.CouponAPI + $"/api/coupon",
                    Data = coupon
                });

                if (response != null && response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    couponResponse = JsonConvert.DeserializeObject<CouponDTO>(content);
                }
            }
            catch (Exception ex)
            {

            }
            return couponResponse;
        }

        public async Task<bool> DeleteCouponAsync(int id)
        {
            var couponDeleted = false;
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Delete,
                    Url = Routes.CouponAPI + $"/api/coupon/{id}"
                });

                if (response != null && response.IsSuccessStatusCode)
                {
                    couponDeleted = true;
                }
            }
            catch (Exception ex)
            {

            }

            return couponDeleted;
        }

        public async Task<List<CouponDTO>> GetAllAsync()
        {
            List<CouponDTO>? coupons = [];
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Get,
                    Url = Routes.CouponAPI + "/api/coupon"
                });

                if (response != null && response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    coupons = JsonConvert.DeserializeObject<List<CouponDTO>>(content);
                }
            }
            catch (Exception ex)
            {

            }
            return coupons ?? [];
        }

        public async Task<CouponDTO?> GetCouponAsync(string couponCode)
        {
            CouponDTO? coupon = null;
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Get,
                    Url = Routes.CouponAPI + $"/api/coupon/code/{couponCode}"
                });

                if (response != null && response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    coupon = JsonConvert.DeserializeObject<CouponDTO>(content);
                }
            }
            catch (Exception ex)
            {

            }
            return coupon;
        }

        public async Task<CouponDTO?> GetCouponByIdAsync(int id)
        {
            CouponDTO? coupon = null;
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Get,
                    Url = Routes.CouponAPI + $"/api/coupon/{id}"
                });

                if (response != null && response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    coupon = JsonConvert.DeserializeObject<CouponDTO>(content);
                }
            }
            catch (Exception ex)
            {

            }
            return coupon;
        }

        public async Task<CouponDTO?> UpdateCouponAsync(CouponDTO coupon)
        {
            CouponDTO? couponResponse = null;
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Put,
                    Url = Routes.CouponAPI + $"/api/coupon",
                    Data = coupon
                });

                if (response != null && response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    couponResponse = JsonConvert.DeserializeObject<CouponDTO>(content);
                }
            }
            catch (Exception ex)
            {

            }
            return couponResponse;
        }
    }
}
