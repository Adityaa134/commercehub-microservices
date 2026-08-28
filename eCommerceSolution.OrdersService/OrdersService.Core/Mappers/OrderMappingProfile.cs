using AutoMapper;
using OrdersService.Core.Domain.Entities;
using OrdersService.Core.DTO;

namespace OrdersService.Core.Mappers
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<OrderAddRequest, Order>();
            CreateMap<OrderItemAddRequest, OrderItem>();

            CreateMap<AddressResponse, Order>()
                .ForMember(dest => dest.ShippingFullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.ShippingPhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.ShippingAddressLine1, opt => opt.MapFrom(src => src.AddressLine1))
                .ForMember(dest => dest.ShippingAddressLine2, opt => opt.MapFrom(src => src.AddressLine2))
                .ForMember(dest => dest.ShippingCity, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.ShippingState, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.ShippingPinCode, opt => opt.MapFrom(src => src.PinCode));

            CreateMap<Order, OrderResponse>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.ToString()));

            CreateMap<OrderItem, OrderItemResponse>()
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));
        }
    }
}
