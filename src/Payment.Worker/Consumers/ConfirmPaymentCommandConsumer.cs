using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Payment.Worker.Interfaces;
using Saga.Contracts;
using System.Diagnostics;

namespace Payment.Worker.Consumers;
public class ConfirmPaymentCommandConsumer : IConsumer<ConfirmPaymentCommand>
{
    private readonly ILogger<ConfirmPaymentCommandConsumer> _logger;
    private readonly IPaymentConfirmedPublisher _paymentConfirmedPublisher;

    public ConfirmPaymentCommandConsumer(ILogger<ConfirmPaymentCommandConsumer> logger, IPaymentConfirmedPublisher paymentConfirmedPublisher)
    {
        _logger = logger;
        _paymentConfirmedPublisher = paymentConfirmedPublisher;
    }

    public async Task Consume(ConsumeContext<ConfirmPaymentCommand> context)
    {
        var timer = Stopwatch.StartNew();

        await Task.Delay(TimeSpan.FromSeconds(8));

        _logger.LogInformation($"Payment confirmed - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<PaymentConfirmedEvent>.ShortName);

        await _paymentConfirmedPublisher.Publish(context.Message.OrderId, context.Message.CustomerName);
    }
}