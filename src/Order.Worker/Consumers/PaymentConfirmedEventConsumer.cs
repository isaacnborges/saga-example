using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Saga.Contracts;
using System.Diagnostics;

namespace Order.Worker.Consumers;
public class PaymentConfirmedEventConsumer : IConsumer<PaymentConfirmedEvent>
{
    private readonly IBus _bus;
    private readonly ILogger<PaymentConfirmedEventConsumer> _logger;

    public PaymentConfirmedEventConsumer(IBus bus, ILogger<PaymentConfirmedEventConsumer> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentConfirmedEvent> context)
    {
        var timer = Stopwatch.StartNew();

        _logger.LogInformation($"Payment confirmed - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<PaymentConfirmedEvent>.ShortName);

        var command = new OrderProcessedEvent(context.Message.OrderId, context.Message.CustomerName, InVar.Timestamp);
        await _bus.Publish(command);
        _logger.LogInformation($"Send OrderProcessedEvent - OrderId: {command.OrderId}");
    }
}