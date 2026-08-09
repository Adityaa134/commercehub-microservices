using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UsersService.Core.Mappers;
using UsersService.Core.Options;
using UsersService.Core.ServiceContracts;
using UsersService.Core.Services;

namespace UsersService.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAddressService, AddressService>();

            services.AddOptions<EmailSettings>()
                .BindConfiguration("EmailSettings");

            services.AddOptions<AuthSettings>()
                .BindConfiguration("AuthSettings");

            services.AddAutoMapper(cfg => { }, typeof(UserMappingProfile).Assembly);

            services.AddSingleton(opt =>
            {
                var configuration = opt.GetRequiredService<IConfiguration>();
                return new BlobServiceClient(configuration["BlobStorage:ConnectionString"]);
            });

            return services;
        }
    }
}
