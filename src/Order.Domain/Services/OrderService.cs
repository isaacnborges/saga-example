using Bogus;
using Bogus.DataSets;
using Bogus.Extensions.Brazil;
using MassTransit;
using Microsoft.Extensions.Logging;
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

    public async Task<Models.Order> CreateOrder()
    {
        var order = new Faker<Models.Order>().CustomInstantiator(x => new Models.Order
        {
            Id = Guid.NewGuid(),
            CustomerName = x.Person.FullName,
            Cnpj = new Company().Cnpj().Replace(".", "").Replace("-", "").Replace("/", ""),
            Status = OrderStatus.Created,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = Guid.NewGuid(),
                    Description = x.Commerce.ProductName(),
                    Quantity = x.Random.Int(1, 99),
                    Value = x.Random.Decimal(50, 999)
                }
            }
        }).Generate();

        var orderStatusHistory = new OrderStatusHistory(order.Id, OrderStatus.Created);

        await _orderRepository.Add(order);
        await _orderStatusHistoryRepository.Add(orderStatusHistory);

        await _bus.Publish(new OrderCreatedEvent(order.Id, order.CustomerName, InVar.Timestamp));

        _logger.LogInformation("Pedido criado");

        return order;
    }
}
