using AutoMapper;
using Orange.Models.DTO;
using Orange.Services.ShoppingCartAPI.Models.Entity;

namespace Orange.Services.ShoppingCartAPI.Mappings
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<CartHeaderDTO, CartHeader>().ReverseMap();
            CreateMap<CartDetailsDTO, CartDetails>().ReverseMap();
        }
    }
}
