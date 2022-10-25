using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Saga.Contracts;
using System.Diagnostics;

namespace Order.Worker.Consumers;
public class IndustryIntegratedEvent2Consumer : IConsumer<IndustryIntegratedEvent2>
{
    private readonly IBus _bus;
    private readonly ILogger<IndustryIntegratedEvent2Consumer> _logger;

    public IndustryIntegratedEvent2Consumer(IBus bus, ILogger<IndustryIntegratedEvent2Consumer> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IndustryIntegratedEvent2> context)
    {
        var timer = Stopwatch.StartNew();

        _logger.LogInformation($"Integration with industry successfully - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<IndustryIntegratedEvent2>.ShortName);

        var command = new ConfirmPaymentCommand2(context.Message.OrderId, context.Message.CustomerName, InVar.Timestamp);
        await _bus.Publish(command);
        _logger.LogInformation($"Send ConfirmPaymentCommand2 - OrderId: {command.OrderId}");
    }
}