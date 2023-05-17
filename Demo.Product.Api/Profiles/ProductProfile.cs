using AutoMapper;

namespace Demo.Product.Api.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            _ = CreateMap<Domain.Product, DTO.Product>().ReverseMap();
        }
    }
}
