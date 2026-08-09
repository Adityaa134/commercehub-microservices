using AutoMapper;
using UsersService.Core.Domain.Entities;
using UsersService.Core.DTO;

namespace UsersService.Core.Mappers
{
    public class AddressMappingProfile : Profile
    {
        public AddressMappingProfile()
        {
            CreateMap<AddAddressRequest, Address>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<UpdateAddressRequest, Address>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.IsDefault, opt => opt.Ignore());

            CreateMap<Address, AddressResponse>()
                .ForMember(dest=>dest.Type,opt=>opt.MapFrom(src=>src.Type.ToString()));
        }
    }
}
