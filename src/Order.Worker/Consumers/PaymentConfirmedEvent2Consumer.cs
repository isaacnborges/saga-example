using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Saga.Contracts;
using System.Diagnostics;

namespace Order.Worker.Consumers;
public class PaymentConfirmedEvent2Consumer : IConsumer<PaymentConfirmedEvent2>
{
    private readonly IBus _bus;
    private readonly ILogger<PaymentConfirmedEvent2Consumer> _logger;

    public PaymentConfirmedEvent2Consumer(IBus bus, ILogger<PaymentConfirmedEvent2Consumer> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentConfirmedEvent2> context)
    {
        var timer = Stopwatch.StartNew();

        _logger.LogInformation($"Payment authorized - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<PaymentConfirmedEvent2>.ShortName);

        await Task.Delay(TimeSpan.FromSeconds(3));

        var command = new OrderProcessedEvent2(context.Message.OrderId, context.Message.CustomerName, InVar.Timestamp);
        await _bus.Publish(command);
        _logger.LogInformation($"Send OrderProcessedEvent2 - OrderId: {command.OrderId}");
    }
}