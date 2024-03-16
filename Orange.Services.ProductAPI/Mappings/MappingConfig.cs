using AutoMapper;
using Orange.Models.DTO;
using Orange.Services.ProductAPI.Models.Entity;

namespace Orange.Services.ProductAPI.Mappings
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<ProductDTO, Product>().ReverseMap();
        }
    }
}
