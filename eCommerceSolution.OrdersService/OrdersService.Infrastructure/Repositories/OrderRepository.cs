using Microsoft.EntityFrameworkCore;
using OrdersService.Core.Domain.Entities;
using OrdersService.Core.Domain.RepositoryContracts;
using OrdersService.Core.DTO;
using OrdersService.Infrastructure.DBContext;

namespace OrdersService.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Order> CreateOrder(Order order)
        {
            await _dbContext.AddAsync(order);
            await _dbContext.SaveChangesAsync();
            return order;
        }

        public async Task<OrderResponse?> GetOrderById(Guid orderId)
        {
            return await _dbContext.Orders
                            .AsNoTracking()
                            .Where(o => o.Id == orderId)
                            .Select(o => new OrderResponse
                            {
                                OrderId = o.Id,
                                OrderDate = o.OrderDate,
                                UserId = o.UserId,
                                AddressId = o.AddressId,
                                TotalPrice = o.TotalPrice,
                                OrderStatus = o.OrderStatus.ToString(),
                                OrderItems = o.OrderItems.Select(oi => new OrderItemResponse
                                {
                                    ProductId = oi.ProductId,
                                    Quantity = oi.Quantity,
                                    UnitPrice = oi.UnitPrice
                                }).ToList()
                            })
                            .FirstOrDefaultAsync();
        }

        public async Task<List<OrderResponse>?> GetOrdersByUserId(Guid userId)
        {
            return await _dbContext.Orders
                            .AsNoTracking()
                            .Where(o=>o.UserId == userId)
                            .OrderByDescending(o=>o.OrderDate)
                            .Select(o => new OrderResponse{
                               OrderId = o.Id, 
                               OrderDate = o.OrderDate,
                               UserId = o.UserId,
                               AddressId = o.AddressId,
                               TotalPrice = o.TotalPrice,
                               OrderStatus = o.OrderStatus.ToString(),
                               OrderItems = o.OrderItems.Select(oi=>new OrderItemResponse
                               {
                                   ProductId = oi.ProductId,
                                   Quantity = oi.Quantity,
                                   UnitPrice = oi.UnitPrice
                               }).ToList()
                            })
                            .ToListAsync();
        }

        public async Task<bool> IsOrderExist(Guid orderId)
        {
            return await _dbContext.Orders.AnyAsync(o => o.Id == orderId);
        }
    }
}
