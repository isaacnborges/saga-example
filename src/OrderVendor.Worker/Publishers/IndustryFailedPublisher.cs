using MassTransit;
using Microsoft.Extensions.Logging;
using OrderVendor.Worker.Publishers.Interfaces;
using Saga.Contracts;

namespace OrderVendor.Worker.Publishers;
public class IndustryFailedPublisher : IIndustryFailedPublisher
{
    private readonly IBus _bus;
    private readonly ILogger<IndustryFailedPublisher> _logger;

    public IndustryFailedPublisher(IBus bus, ILogger<IndustryFailedPublisher> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task Publish(Guid orderId, string customerName)
    {
        var @event = new IndustryFailedEvent(orderId, customerName, InVar.Timestamp);
        await _bus.Publish(@event);
        _logger.LogInformation($"Send IndustryFailedEvent - OrderId: {@event.OrderId}");
    }
}
