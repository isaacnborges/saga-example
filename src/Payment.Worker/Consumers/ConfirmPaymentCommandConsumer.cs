using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Saga.Contracts;
using System.Diagnostics;

namespace Payment.Worker.Consumers;
public class ConfirmPaymentCommandConsumer : IConsumer<ConfirmPaymentCommand>
{
    private readonly IBus _bus;
    private readonly ILogger<ConfirmPaymentCommandConsumer> _logger;

    public ConfirmPaymentCommandConsumer(IBus bus, ILogger<ConfirmPaymentCommandConsumer> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ConfirmPaymentCommand> context)
    {
        var timer = Stopwatch.StartNew();

        _logger.LogInformation($"Payment confirmed - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<PaymentAuthorizedEvent>.ShortName);

        var paymentAuthorizedEvent = new PaymentConfirmedEvent(context.Message.OrderId, context.Message.CustomerName);
        await _bus.Publish(paymentAuthorizedEvent);
        _logger.LogInformation($"Send PaymentConfirmedEvent - OrderId: {paymentAuthorizedEvent.OrderId}");
    }
}