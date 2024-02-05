using AutoMapper;
using Orange.Models.DTO;
using Orange.Services.CouponAPI.Models.Entity;

namespace Orange.Services.CouponAPI.Mappings
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<CouponDTO, Coupon>().ReverseMap();
        }
    }
}
