using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Order.Worker.Interfaces;
using Saga.Contracts;
using System.Diagnostics;

namespace Order.Worker.Consumers;
public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventConsumer> _logger;
    private readonly IAuthorizePaymentPublisher _authorizePaymentPublisher;

    public OrderCreatedEventConsumer(ILogger<OrderCreatedEventConsumer> logger, IAuthorizePaymentPublisher authorizePaymentPublisher)
    {
        _logger = logger;
        _authorizePaymentPublisher = authorizePaymentPublisher;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var timer = Stopwatch.StartNew();

        _logger.LogInformation($"Order processed successfully - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<OrderCreatedEvent>.ShortName);

        await _authorizePaymentPublisher.Publish(context.Message.OrderId, context.Message.CustomerName);
    }
}
