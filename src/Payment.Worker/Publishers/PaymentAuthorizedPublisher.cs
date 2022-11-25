using MassTransit;
using Microsoft.Extensions.Logging;
using Payment.Worker.Interfaces;
using Saga.Contracts;

namespace Payment.Worker.Publishers;
public class PaymentAuthorizedPublisher : IPaymentAuthorizedPublisher
{
    private readonly IBus _bus;
    private readonly ILogger<PaymentAuthorizedPublisher> _logger;

    public PaymentAuthorizedPublisher(IBus bus, ILogger<PaymentAuthorizedPublisher> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task Publish(Guid orderId, string customerName)
    {
        var @event = new PaymentAuthorizedEvent(orderId, customerName, InVar.Timestamp);
        await _bus.Publish(@event);
        _logger.LogInformation($"Send PaymentAuthorizedEvent - OrderId: {@event.OrderId}");
    }
}
