using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Saga.Contracts;
using System.Diagnostics;

namespace Payment.Worker.Consumers;
public class AuthorizePaymentCommand2Consumer : IConsumer<AuthorizePaymentCommand2>
{
    private readonly IBus _bus;
    private readonly ILogger<AuthorizePaymentCommandConsumer> _logger;

    public AuthorizePaymentCommand2Consumer(IBus bus, ILogger<AuthorizePaymentCommandConsumer> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<AuthorizePaymentCommand2> context)
    {
        var timer = Stopwatch.StartNew();

        await Task.Delay(TimeSpan.FromSeconds(2));

        _logger.LogInformation($"Payment authorized - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<PaymentAuthorizedEvent2>.ShortName);

        var @event = new PaymentAuthorizedEvent2(context.Message.OrderId, context.Message.CustomerName, InVar.Timestamp);
        await _bus.Publish(@event);
        _logger.LogInformation($"Send PaymentAuthorizedEvent2 - OrderId: {@event.OrderId}");
    }
}