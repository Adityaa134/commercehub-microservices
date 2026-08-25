using AutoMapper;
using UsersService.Core.Domain.Entities;
using UsersService.Core.DTO;

namespace UsersService.Core.Mappers
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<ApplicationUser, UserProfileResponse>()
                .ForMember(des => des.UserId, opt => opt.MapFrom(src => src.Id));

            CreateMap<RegisterRequest, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.FullName));
        }
    }
}
