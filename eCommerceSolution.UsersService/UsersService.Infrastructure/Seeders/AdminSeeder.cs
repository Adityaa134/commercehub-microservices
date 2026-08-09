using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UsersService.Core.Constants;
using UsersService.Core.Domain.Entities;

namespace UsersService.Infrastructure.Seeders
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider services, IConfiguration config)
        {
            ILoggerFactory loggerFactory = services.GetRequiredService<ILoggerFactory>();
            ILogger logger = loggerFactory.CreateLogger(typeof(AdminSeeder));
            RoleManager<ApplicationRole> roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
            UserManager<ApplicationUser> userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            foreach (var role in Roles.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                   IdentityResult roleResult = await roleManager.CreateAsync(new ApplicationRole { Name = role });
                   if (roleResult.Succeeded)
                   {
                       logger.LogInformation($"Role '{role}' created successfully.");
                   }
                   else
                   {
                       string errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                       logger.LogError($"Failed to create role '{role}': {errors}");
                   }
                }  
            }

            string? adminEmail = config["SeedAdmin:Email"];
            string? adminPassword = config["SeedAdmin:Password"];
            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            {
                logger.LogWarning("Admin seeding skipped: SeedAdmin:Email or SeedAdmin:Password configuration is missing.");
                return;
            }
            ApplicationUser? existingAdmin = await userManager.FindByEmailAsync(adminEmail);

            if (existingAdmin == null)
            {
                ApplicationUser adminUser = new ApplicationUser
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    EmailConfirmed = true
                };
                IdentityResult createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, Roles.Admin);
                    logger.LogInformation($"Admin user '{adminEmail}' created successfully and assigned Admin role.");
                }
                else
                {
                    string errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    logger.LogError($"Failed to create admin user '{adminEmail}': {errors}");
                }
            }
        }
    }
}
