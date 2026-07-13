using Microsoft.EntityFrameworkCore;
using OrdersService.Core.Domain.Entities;

namespace OrdersService.Infrastructure.DBContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>(order =>
            {
                order.ToTable("Orders");
                order.HasIndex(o => new { o.UserId, o.OrderDate });
            });
            modelBuilder.Entity<OrderItem>().ToTable("OrderItems");
        }
    }
}
