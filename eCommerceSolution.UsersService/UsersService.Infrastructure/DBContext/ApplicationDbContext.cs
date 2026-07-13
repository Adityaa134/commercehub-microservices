using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UsersService.Core.Domain.Entities;

namespace UsersService.Infrastructure.DBContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,Guid>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Address>(address =>
            {
                address.ToTable("Addresses");
                address.HasIndex(a => a.UserId);
            });

            builder.Entity<RefreshToken>(rt =>
            {
                rt.ToTable("RefreshTokens");
                rt.HasIndex(rt => rt.Token)
                  .IsUnique();
                rt.HasIndex(rt => rt.UserId);
                rt.HasIndex(rt => rt.ExpiresAt);
            });
        }
    }
}
