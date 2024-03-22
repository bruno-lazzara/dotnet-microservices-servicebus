using Orange.Models.DTO;

namespace Orange.Services.OrderAPI.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProducts();
    }
}
