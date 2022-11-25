using MassTransit;
using Microsoft.Extensions.Logging;
using Order.Worker.Interfaces;
using Saga.Contracts;

namespace Order.Worker.Publishers;
public class OrderProcessedPublisher : IOrderProcessedPublisher
{
    private readonly IBus _bus;
    private readonly ILogger<OrderProcessedPublisher> _logger;

    public OrderProcessedPublisher(IBus bus, ILogger<OrderProcessedPublisher> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task Publish(Guid orderId, string customerName)
    {
        var command = new OrderProcessedEvent(orderId, customerName, InVar.Timestamp);
        await _bus.Publish(command);
        _logger.LogInformation($"Send OrderProcessedEvent - OrderId: {command.OrderId}");
    }
}
