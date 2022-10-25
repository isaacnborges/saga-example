using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Saga.Contracts;
using System.Diagnostics;

namespace OrderVendor.Worker.Consumers;
public class IntegrateIndustryCommandConsumer : IConsumer<IntegrateIndustryCommand>
{
    private readonly IBus _bus;
    private readonly ILogger<IntegrateIndustryCommandConsumer> _logger;

    public IntegrateIndustryCommandConsumer(IBus bus, ILogger<IntegrateIndustryCommandConsumer> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IntegrateIndustryCommand> context)
    {
        var timer = Stopwatch.StartNew();

        _logger.LogInformation($"Industry integreated - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<IndustryIntegratedEvent>.ShortName);

        var paymentAuthorizedEvent = new IndustryIntegratedEvent(context.Message.OrderId, context.Message.CustomerName);
        await _bus.Publish(paymentAuthorizedEvent);
        _logger.LogInformation($"Send IndustryIntegratedEvent - OrderId: {paymentAuthorizedEvent.OrderId}");
    }
}
