using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using OrdersService.Core.Domain.RepositoryContracts;
using OrdersService.Core.ServiceContracts;
using OrdersService.Infrastructure.HttpClients;
using OrdersService.Infrastructure.Repositories;
using OrdersService.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;

namespace OrdersService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddHttpContextAccessor();

            services.AddTransient<BearerTokenPropagationHandler>();

            services.AddHttpClient<IUserServiceClient, UserServiceClient>(client =>
            {
                client.BaseAddress = new Uri(
                    configuration["UserService:BaseUrl"]!);
            })
            .AddHttpMessageHandler<BearerTokenPropagationHandler>();

            services.AddHttpClient<IProductServiceClient, ProductServiceClient>(client =>
            {
                client.BaseAddress = new Uri(
                    configuration["ProductService:BaseUrl"]!);
            })
            .AddHttpMessageHandler<BearerTokenPropagationHandler>();

            return services;
        }
    }
}
