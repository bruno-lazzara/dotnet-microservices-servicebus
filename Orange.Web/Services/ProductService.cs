using Newtonsoft.Json;
using Orange.Models;
using Orange.Models.DTO;
using Orange.Web.Services.Interfaces;
using Orange.Web.Utils;

namespace Orange.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;
        public ProductService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ProductDTO?> CreateProductAsync(ProductDTO product)
        {
            ProductDTO? productResponse = null;
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Post,
                    Url = Routes.ProductAPI + $"/api/product",
                    Data = product,
                    ContentType = ContentType.MultipartFormData
                });

                if (response != null && response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    productResponse = JsonConvert.DeserializeObject<ProductDTO>(content);
                }
            }
            catch (Exception ex)
            {

            }
            return productResponse;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var productDeleted = false;
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Delete,
                    Url = Routes.ProductAPI + $"/api/product/{id}"
                });

                if (response != null && response.IsSuccessStatusCode)
                {
                    productDeleted = true;
                }
            }
            catch (Exception ex)
            {

            }

            return productDeleted;
        }

        public async Task<List<ProductDTO>> GetAllAsync()
        {
            List<ProductDTO>? products = [];
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Get,
                    Url = Routes.ProductAPI + "/api/product"
                });

                if (response != null && response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    products = JsonConvert.DeserializeObject<List<ProductDTO>>(content);
                }
            }
            catch (Exception ex)
            {

            }
            return products ?? [];
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            ProductDTO? product = null;
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Get,
                    Url = Routes.ProductAPI + $"/api/product/{id}"
                });

                if (response != null && response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    product = JsonConvert.DeserializeObject<ProductDTO>(content);
                }
            }
            catch (Exception ex)
            {

            }
            return product;
        }

        public async Task<ProductDTO?> UpdateProductAsync(ProductDTO product)
        {
            ProductDTO? productResponse = null;
            try
            {
                var response = await _baseService.SendAsync(new RequestDTO
                {
                    HttpMethod = HttpMethod.Put,
                    Url = Routes.ProductAPI + $"/api/product",
                    Data = product
                });

                if (response != null && response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    productResponse = JsonConvert.DeserializeObject<ProductDTO>(content);
                }
            }
            catch (Exception ex)
            {

            }
            return productResponse;
        }
    }
}
