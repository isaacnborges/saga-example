using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Saga.Contracts;
using System.Diagnostics;

namespace Order.Worker.Consumers;
public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventConsumer> _logger;
    private readonly IBus _bus;

    public OrderCreatedEventConsumer(ILogger<OrderCreatedEventConsumer> logger, IBus bus)
    {
        _logger = logger;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var timer = Stopwatch.StartNew();

        _logger.LogInformation($"Order processed successfully - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<OrderCreatedEvent>.ShortName);

        var command = new AuthorizePaymentCommand(context.Message.OrderId, context.Message.CustomerName, InVar.Timestamp);
        await _bus.Publish(command);
        _logger.LogInformation($"Send AuthorizePaymentCommand - OrderId: {command.OrderId}");
    }
}
