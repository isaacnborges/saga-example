using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Order.Domain.Interfaces;
using Order.Domain.Models;
using Order.Worker.Interfaces;
using Saga.Contracts;
using System.Diagnostics;

namespace Order.Worker.Consumers;
public class PaymentConfirmedEventConsumer : IConsumer<PaymentConfirmedEvent>
{
    private readonly ILogger<PaymentConfirmedEventConsumer> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderStatusHistoryRepository _orderStatusHistoryRepository;
    private readonly IOrderProcessedPublisher _orderProcessedPublisher;

    public PaymentConfirmedEventConsumer(
        ILogger<PaymentConfirmedEventConsumer> logger,
        IOrderRepository orderRepository,
        IOrderStatusHistoryRepository orderStatusHistoryRepository,
        IOrderProcessedPublisher orderProcessedPublisher)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _orderStatusHistoryRepository = orderStatusHistoryRepository;
        _orderProcessedPublisher = orderProcessedPublisher;
    }

    public async Task Consume(ConsumeContext<PaymentConfirmedEvent> context)
    {
        var timer = Stopwatch.StartNew();

        await Task.Delay(TimeSpan.FromSeconds(5));

        _logger.LogInformation($"Payment confirmed - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<PaymentConfirmedEvent>.ShortName);

        await UpdateOrder(context);

        await _orderProcessedPublisher.Publish(context.Message.OrderId, context.Message.CustomerName);
    }

    private async Task UpdateOrder(ConsumeContext<PaymentConfirmedEvent> context)
    {
        var order = await _orderRepository.GetById(context.Message.OrderId);
        order.UpdateStatus(OrderStatus.PaymentConfirmed);

        var orderStatusHistory = new OrderStatusHistory(order.Id, OrderStatus.PaymentConfirmed);

        await _orderRepository.Update(order);
        await _orderStatusHistoryRepository.Add(orderStatusHistory);
    }
}