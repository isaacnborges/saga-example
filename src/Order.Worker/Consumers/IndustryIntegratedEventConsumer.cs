using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Saga.Contracts;
using System.Diagnostics;

namespace Order.Worker.Consumers;
public class IndustryIntegratedEventConsumer : IConsumer<IndustryIntegratedEvent>
{
    private readonly IBus _bus;
    private readonly ILogger<IndustryIntegratedEventConsumer> _logger;

    public IndustryIntegratedEventConsumer(IBus bus, ILogger<IndustryIntegratedEventConsumer> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IndustryIntegratedEvent> context)
    {
        var timer = Stopwatch.StartNew();

        _logger.LogInformation($"Integration with industry successfully - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<IndustryIntegratedEvent>.ShortName);

        var command = new ConfirmPaymentCommand(context.Message.OrderId, context.Message.CustomerName, InVar.Timestamp);
        await _bus.Publish(command);
        _logger.LogInformation($"Send ConfirmPaymentCommand - OrderId: {command.OrderId}");
    }
}