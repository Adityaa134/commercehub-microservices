using Microsoft.EntityFrameworkCore;
using ProductsService.Core.Domain.Entities;

namespace ProductsService.Infrastructure.DBContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(product =>
            {
                product.ToTable("Products");
                product.HasIndex(p => p.Name);
            });

            modelBuilder.Entity<Category>(catgeory =>
            {
                catgeory.ToTable("Categories");
                catgeory.HasIndex(c => c.Name)
                        .IsUnique();
                catgeory.Property(c => c.DisplayOrder)
                        .HasDefaultValue(0);
                catgeory.HasOne(c => c.ParentCategory)
                        .WithMany(c => c.SubCategories)
                        .HasForeignKey(c => c.ParentCategoryId)
                        .OnDelete(DeleteBehavior.Restrict);
                catgeory.HasIndex(c => c.ParentCategoryId);
            });

            modelBuilder.Entity<Cart>(cart =>
            {
                cart.ToTable("Carts");
                cart.HasIndex(c => c.UserId)
                    .IsUnique();
            });

            modelBuilder.Entity<CartItem>(cartItem =>
            {
                cartItem.ToTable("CartItems");
                cartItem.HasIndex(c => new { c.CartId , c.ProductId})
                        .IsUnique();
            });

            modelBuilder.Entity<Rating>(rating =>
            {
                rating.ToTable("Ratings");
                rating.HasIndex(r => new { r.OrderId , r.ProductId})
                      .IsUnique();
            });
        }
    }
}
