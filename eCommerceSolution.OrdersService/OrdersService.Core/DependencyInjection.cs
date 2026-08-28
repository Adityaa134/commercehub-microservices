using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrdersService.Core.Mappers;
using OrdersService.Core.ServiceContracts;
using OrdersService.Core.Services;

namespace OrdersService.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IOrderService, OrderService>();

            services.AddAutoMapper(cfg => { },typeof(OrderMappingProfile).Assembly);

            return services;
        }
    }
}
