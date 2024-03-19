using Orange.Models.DTO;

namespace Orange.Services.ShoppingCartAPI.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProducts();
    }
}
