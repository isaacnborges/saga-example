using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Saga.Contracts;
using System.Diagnostics;

namespace OrderVendor.Worker.Consumers;
public class IntegrateIndustryCommand2Consumer : IConsumer<IntegrateIndustryCommand2>
{
    private readonly IBus _bus;
    private readonly ILogger<IntegrateIndustryCommand2Consumer> _logger;

    public IntegrateIndustryCommand2Consumer(IBus bus, ILogger<IntegrateIndustryCommand2Consumer> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IntegrateIndustryCommand2> context)
    {
        var timer = Stopwatch.StartNew();

        await Task.Delay(TimeSpan.FromSeconds(10));

        _logger.LogInformation($"Industry integreated - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<IndustryIntegratedEvent>.ShortName);

        var @event = new IndustryIntegratedEvent2(context.Message.OrderId, context.Message.CustomerName, InVar.Timestamp);
        await _bus.Publish(@event);
        _logger.LogInformation($"Send IndustryIntegratedEvent2 - OrderId: {@event.OrderId}");
    }
}
