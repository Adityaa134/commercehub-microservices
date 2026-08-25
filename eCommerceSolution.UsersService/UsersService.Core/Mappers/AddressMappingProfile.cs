using AutoMapper;
using UsersService.Core.Domain.Entities;
using UsersService.Core.DTO;

namespace UsersService.Core.Mappers
{
    public class AddressMappingProfile : Profile
    {
        public AddressMappingProfile()
        {
            CreateMap<AddAddressRequest, Address>();
            CreateMap<UpdateAddressRequest, Address>();

            CreateMap<Address, AddressResponse>()
                .ForMember(dest => dest.AddressId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
        }
    }
}
