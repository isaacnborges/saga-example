using MongoDB.Driver;
using Order.Domain.Interfaces;
using Saga.Core.Contexts;

namespace Order.Domain.Infra.Repositories;
public class OrderRepository : IOrderRepository
{
    protected readonly IMongoCollection<Models.Order> _orders;

    public OrderRepository(IMongoContext context)
    {
        _orders = context.GetCollection<Models.Order>("orders");
    }

    public async Task Add(Models.Order order) => await _orders.InsertOneAsync(order);

    public async Task Update(Models.Order order) => await _orders.ReplaceOneAsync(t => t.Id == order.Id, order);

    public async Task<Models.Order> GetById(Guid id)
    {
        var order = await _orders
            .Find(t => t.Id == id)
            .FirstOrDefaultAsync();

        return order;
    }
}