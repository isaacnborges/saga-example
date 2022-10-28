using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Saga.Contracts;
using System.Diagnostics;

namespace Payment.Worker.Consumers;
public class AuthorizePaymentCommandConsumer : IConsumer<AuthorizePaymentCommand>
{
    private readonly IBus _bus;
    private readonly ILogger<AuthorizePaymentCommandConsumer> _logger;

    public AuthorizePaymentCommandConsumer(IBus bus, ILogger<AuthorizePaymentCommandConsumer> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<AuthorizePaymentCommand> context)
    {
        var timer = Stopwatch.StartNew();

        await Task.Delay(TimeSpan.FromSeconds(5));

        _logger.LogInformation($"Payment authorized - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<AuthorizePaymentCommand>.ShortName);

        var @event = new PaymentAuthorizedEvent(context.Message.OrderId, context.Message.CustomerName, InVar.Timestamp);
        await _bus.Publish(@event);
        _logger.LogInformation($"Send PaymentAuthorizedEvent - OrderId: {@event.OrderId}");
    }
}