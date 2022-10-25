using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Saga.Contracts;
using System.Diagnostics;

namespace Payment.Worker.Consumers;
public class ConfirmPaymentCommand2Consumer : IConsumer<ConfirmPaymentCommand2>
{
    private readonly IBus _bus;
    private readonly ILogger<ConfirmPaymentCommand2Consumer> _logger;

    public ConfirmPaymentCommand2Consumer(IBus bus, ILogger<ConfirmPaymentCommand2Consumer> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ConfirmPaymentCommand2> context)
    {
        var timer = Stopwatch.StartNew();

        await Task.Delay(TimeSpan.FromSeconds(5));

        _logger.LogInformation($"Payment confirmed - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<PaymentConfirmedEvent2>.ShortName);

        var @event = new PaymentConfirmedEvent2(context.Message.OrderId, context.Message.CustomerName, InVar.Timestamp);
        await _bus.Publish(@event);
        _logger.LogInformation($"Send PaymentConfirmedEvent - OrderId: {@event.OrderId}");
    }
}