using MassTransit;
using Microsoft.Extensions.Logging;
using Payment.Worker.Interfaces;
using Saga.Contracts;

namespace Payment.Worker.Publishers;
public class PaymentConfirmedPublisher : IPaymentConfirmedPublisher
{
    private readonly IBus _bus;
    private readonly ILogger<PaymentConfirmedPublisher> _logger;

    public PaymentConfirmedPublisher(IBus bus, ILogger<PaymentConfirmedPublisher> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task Publish(Guid orderId, string customerName)
    {
        var @event = new PaymentConfirmedEvent(orderId, customerName, InVar.Timestamp);
        await _bus.Publish(@event);
        _logger.LogInformation($"Send PaymentConfirmedEvent - OrderId: {@event.OrderId}");
    }
}