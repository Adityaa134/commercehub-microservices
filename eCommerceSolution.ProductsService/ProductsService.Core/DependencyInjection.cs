using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductsService.Core.Mappers;
using ProductsService.Core.ServiceContracts;
using ProductsService.Core.Services;

namespace ProductsService.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IImageService,ImageService>();

            services.AddAutoMapper(cfg=> { }, typeof(ProductMappingProfile).Assembly);

            services.AddSingleton(opt =>
            {
                var configuration = opt.GetRequiredService<IConfiguration>();
                return new BlobServiceClient(configuration["BlobStorage:ConnectionString"]);
            });

            return services;
        }
    }
}
