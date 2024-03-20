using Newtonsoft.Json;
using Orange.Models.DTO;
using Orange.Web.Services.Interfaces;
using Orange.Web.Utils;

namespace Orange.Web.Services
{
    public class CartService : ICartService
    {
        private readonly IBaseService _baseService;
        public CartService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<bool> ApplyCouponAsync(CartDTO cart)
        {
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Post,
                    Url = Routes.CartAPI + $"/api/cart/ApplyCoupon",
                    Data = cart
                });

                return response != null && response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> EmailCartAsync(CartDTO cart)
        {
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Post,
                    Url = Routes.CartAPI + $"/api/cart/EmailCartRequest",
                    Data = cart
                });

                return response != null && response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<CartDTO?> GetCartByUserIdAsync(string userId)
        {
            CartDTO? cart = null;
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Get,
                    Url = Routes.CartAPI + $"/api/cart/GetCart/{userId}"
                });

                if (response != null && response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    cart = JsonConvert.DeserializeObject<CartDTO>(content);
                }
            }
            catch (Exception ex)
            {

            }
            return cart;
        }

        public async Task<bool> RemoveFromCartAsync(int cartDetailsId)
        {
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Post,
                    Url = Routes.CartAPI + $"/api/cart/RemoveCartDetails",
                    Data = cartDetailsId
                });

                return response != null && response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<CartDTO?> UpsertCartAsync(CartDTO cart)
        {
            CartDTO? cartResponse = null;
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Post,
                    Url = Routes.CartAPI + $"/api/cart/CartUpsert",
                    Data = cart
                });

                if (response != null && response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    cartResponse = JsonConvert.DeserializeObject<CartDTO>(content);
                }
            }
            catch (Exception ex)
            {

            }
            return cartResponse;
        }
    }
}
