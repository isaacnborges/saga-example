using MassTransit;
using Microsoft.Extensions.Logging;
using OrderVendor.Worker.Publishers.Interfaces;
using Saga.Contracts;

namespace OrderVendor.Worker.Publishers;
public class IndustryIntegratedPublisher : IIndustryIntegratedPublisher
{
    private readonly IBus _bus;
    private readonly ILogger<IndustryIntegratedPublisher> _logger;

    public IndustryIntegratedPublisher(IBus bus, ILogger<IndustryIntegratedPublisher> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task Publish(Guid orderId, string customerName)
    {
        var @event = new IndustryIntegratedEvent(orderId, customerName, InVar.Timestamp);
        await _bus.Publish(@event);
        _logger.LogInformation($"Send IndustryIntegratedEvent - OrderId: {@event.OrderId}");
    }
}
