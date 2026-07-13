using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using OrdersService.Core.Domain.RepositoryContracts;
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

            return services;
        }
    }
}
