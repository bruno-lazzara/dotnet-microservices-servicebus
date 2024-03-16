using Orange.Models.DTO;

namespace Orange.Web.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDTO>> GetAllAsync();
        Task<ProductDTO?> GetProductByIdAsync(int id);
        Task<ProductDTO?> CreateProductAsync(ProductDTO product);
        Task<ProductDTO?> UpdateProductAsync(ProductDTO product);
        Task<bool> DeleteProductAsync(int id);
    }
}
