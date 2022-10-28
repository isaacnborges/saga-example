using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Saga.Contracts;
using System.Diagnostics;

namespace Order.Worker.Consumers;
public class PaymentAuthorizedEventConsumer : IConsumer<PaymentAuthorizedEvent>
{
    private readonly IBus _bus;
    private readonly ILogger<PaymentAuthorizedEventConsumer> _logger;

    public PaymentAuthorizedEventConsumer(IBus bus, ILogger<PaymentAuthorizedEventConsumer> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentAuthorizedEvent> context)
    {
        var timer = Stopwatch.StartNew();

        _logger.LogInformation($"Payment authorized - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<PaymentAuthorizedEvent>.ShortName);

        var command = new IntegrateIndustryCommand(context.Message.OrderId, context.Message.CustomerName, InVar.Timestamp);
        await _bus.Publish(command);
        _logger.LogInformation($"Send IntegrateIndustryCommand - OrderId: {command.OrderId}");
    }
}