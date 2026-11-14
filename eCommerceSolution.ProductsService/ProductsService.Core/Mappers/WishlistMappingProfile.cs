using AutoMapper;
using ProductsService.Core.Domain.Entities;
using ProductsService.Core.DTO;

namespace ProductsService.Core.Mappers
{
    public class WishlistMappingProfile : Profile
    {
        public WishlistMappingProfile()
        {
            CreateMap<WishlistItem, WishlistItemResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Product.UnitPrice))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Product.Category != null ? src.Product.Category.Name : null))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Product.AverageRating))
                .ForMember(dest => dest.TotalRatings, opt => opt.MapFrom(src => src.Product.TotalRatings))
                .ForMember(dest => dest.InStock, opt => opt.MapFrom(src => src.Product.QuantityInStock > 0 && src.Product.IsActive))
                .ForMember(dest => dest.ProductImagePath, opt => opt.MapFrom(src => src.Product.ProductImagePath));
        }
    }
}

