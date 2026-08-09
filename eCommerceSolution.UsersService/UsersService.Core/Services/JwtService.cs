using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UsersService.Core.Domain.Entities;
using UsersService.Core.ServiceContracts;

namespace UsersService.Core.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateJwtToken(ApplicationUser user,IList<string> roles)
        {
            if(user == null)
                throw new ArgumentNullException("User cannot be null");

            DateTime expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpirationTime"]));

            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email ?? string.Empty)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecurityKey"]));

            SigningCredentials signingCredentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
                );

            JwtSecurityToken securityToken = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials:signingCredentials
                );

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string accessToken = tokenHandler.WriteToken(securityToken);

            return accessToken;
        }
    }
}
