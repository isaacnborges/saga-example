using MongoDB.Driver;
using Order.Domain.Interfaces;
using Order.Domain.Models;
using Saga.Core.Contexts;

namespace Order.Domain.Infra.Repositories;
public class OrderStatusHistoryRepository : IOrderStatusHistoryRepository
{
    protected readonly IMongoCollection<OrderStatusHistory> _collection;

    public OrderStatusHistoryRepository(IMongoContext context)
    {
        _collection = context.GetCollection<OrderStatusHistory>("orderStatusHistories");
    }

    public async Task Add(OrderStatusHistory orderStatusHistory) 
        => await _collection.InsertOneAsync(orderStatusHistory);
}