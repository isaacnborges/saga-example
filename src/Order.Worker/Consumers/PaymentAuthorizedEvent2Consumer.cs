using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Saga.Contracts;
using System.Diagnostics;

namespace Order.Worker.Consumers;
public class PaymentAuthorizedEvent2Consumer : IConsumer<PaymentAuthorizedEvent2>
{
    private readonly IBus _bus;
    private readonly ILogger<PaymentAuthorizedEvent2Consumer> _logger;

    public PaymentAuthorizedEvent2Consumer(IBus bus, ILogger<PaymentAuthorizedEvent2Consumer> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentAuthorizedEvent2> context)
    {
        var timer = Stopwatch.StartNew();

        _logger.LogInformation($"Payment authorized - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<PaymentAuthorizedEvent2>.ShortName);

        var command = new IntegrateIndustryCommand2(context.Message.OrderId, context.Message.CustomerName, InVar.Timestamp);
        await _bus.Publish(command);
        _logger.LogInformation($"Send PaymentAuthorizedEvent2 - OrderId: {command.OrderId}");
    }
}