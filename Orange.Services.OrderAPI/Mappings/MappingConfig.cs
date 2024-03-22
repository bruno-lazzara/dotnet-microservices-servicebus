using AutoMapper;
using Orange.Models.DTO;
using Orange.Services.OrderAPI.Models.Entity;

namespace Orange.Services.OrderAPI.Mappings
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<OrderHeaderDTO, OrderHeader>().ReverseMap();
            CreateMap<OrderDetailsDTO, OrderDetails>().ReverseMap();

            CreateMap<OrderHeaderDTO, CartHeaderDTO>()
                .ForMember(dest => dest.CartTotal, opt => opt.MapFrom(src => src.OrderTotal))
                .ReverseMap();

            CreateMap<CartDetailsDTO, OrderDetailsDTO>().ReverseMap();
        }
    }
}
