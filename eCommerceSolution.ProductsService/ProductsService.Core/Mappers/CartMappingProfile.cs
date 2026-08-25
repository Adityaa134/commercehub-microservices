using AutoMapper;
using ProductsService.Core.Domain.Entities;
using ProductsService.Core.DTO;

namespace ProductsService.Core.Mappers
{
    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {
            CreateMap<CartItemAddRequest, CartItem>();
            CreateMap<CartItem, CartItemResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImagePath, opt => opt.Ignore())
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Product.UnitPrice))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.Product.UnitPrice));
        }
    }
}
