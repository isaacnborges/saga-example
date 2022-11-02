using Bogus;
using MassTransit;
using MassTransit.MongoDbIntegration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Order.Domain.Interfaces;
using Order.Domain.Models;
using Saga.Contracts;

namespace Order.Domain.Services;
public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly IBus _bus;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderStatusHistoryRepository _orderStatusHistoryRepository;

    public OrderService(
        ILogger<OrderService> logger, 
        IBus bus,
        IOrderRepository orderRepository,
        IOrderStatusHistoryRepository orderStatusHistoryRepository)
    {
        _logger = logger;
        _bus = bus;
        _orderRepository = orderRepository;
        _orderStatusHistoryRepository = orderStatusHistoryRepository;
    }

    public async Task CreateOrder()
    {
        var order = new Faker<Models.Order>().CustomInstantiator(x => new Models.Order
        {
            Id = Guid.NewGuid(),
            CustomerName = x.Person.FullName,
            Status = OrderStatus.Created
        }).Generate();

        var orderStatusHistory = new OrderStatusHistory(order.Id, OrderStatus.Created);

        await _orderRepository.Add(order);
        await _orderStatusHistoryRepository.Add(orderStatusHistory);

        _logger.LogInformation("Pedido criado");

        await _bus.Publish(new OrderCreatedEvent(order.Id, order.CustomerName, InVar.Timestamp));
    }
}
