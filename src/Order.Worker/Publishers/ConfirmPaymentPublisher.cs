using MassTransit;
using Microsoft.Extensions.Logging;
using Order.Worker.Consumers;
using Order.Worker.Interfaces;
using Saga.Contracts;

namespace Order.Worker.Publishers;
public class ConfirmPaymentPublisher : IConfirmPaymentPublisher
{
    private readonly IBus _bus;
    private readonly ILogger<IndustryIntegratedEventConsumer> _logger;

    public ConfirmPaymentPublisher(IBus bus, ILogger<IndustryIntegratedEventConsumer> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task Publish(Guid orderId, string customerName)
    {
        var command = new ConfirmPaymentCommand(orderId, customerName, InVar.Timestamp);
        await _bus.Publish(command);
        _logger.LogInformation($"Send ConfirmPaymentCommand - OrderId: {command.OrderId}");
    }
}
