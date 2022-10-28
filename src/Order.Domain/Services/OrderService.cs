using Bogus;
using MassTransit;
using Microsoft.Extensions.Logging;
using Saga.Contracts;

namespace Order.Domain.Services;
public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly IBus _bus;

    public OrderService(ILogger<OrderService> logger, IBus bus)
    {
        _logger = logger;
        _bus = bus;
    }

    public async Task CreateOrder()
    {
        var order = new Faker<Models.Order>().CustomInstantiator(x => new Models.Order(Guid.NewGuid(), x.Person.FullName)).Generate();

        _logger.LogInformation("Pedido criado");

        await _bus.Publish(new OrderCreatedEvent(order.OrderId, order.CustomerName, InVar.Timestamp));
    }
}
