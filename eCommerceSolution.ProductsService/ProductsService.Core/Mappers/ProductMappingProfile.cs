using AutoMapper;
using ProductsService.Core.Domain.Entities;
using ProductsService.Core.DTO;

namespace ProductsService.Core.Mappers
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<ProductAddRequest, Product>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProductName.Trim()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description.Trim()));

            CreateMap<ProductUpdateRequest, Product>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProductName.Trim()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description.Trim()));

            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));
        }
    }
}
