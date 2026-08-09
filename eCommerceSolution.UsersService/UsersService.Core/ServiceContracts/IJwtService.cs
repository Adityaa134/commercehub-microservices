using UsersService.Core.Domain.Entities;

namespace UsersService.Core.ServiceContracts
{
    public interface IJwtService
    {
        string CreateJwtToken(ApplicationUser user,IList<string> roles);
    }
}
