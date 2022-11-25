using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Order.Domain.Interfaces;
using Order.Domain.Models;
using Order.Worker.Interfaces;
using Saga.Contracts;
using System.Diagnostics;

namespace Order.Worker.Consumers;
public class IndustryIntegratedEventConsumer : IConsumer<IndustryIntegratedEvent>
{
    private readonly ILogger<IndustryIntegratedEventConsumer> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderStatusHistoryRepository _orderStatusHistoryRepository;
    private readonly IConfirmPaymentPublisher _confirmPaymentPublisher;

    public IndustryIntegratedEventConsumer(
        ILogger<IndustryIntegratedEventConsumer> logger,
        IOrderRepository orderRepository,
        IOrderStatusHistoryRepository orderStatusHistoryRepository,
        IConfirmPaymentPublisher confirmPaymentPublisher)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _orderStatusHistoryRepository = orderStatusHistoryRepository;
        _confirmPaymentPublisher = confirmPaymentPublisher;
    }

    public async Task Consume(ConsumeContext<IndustryIntegratedEvent> context)
    {
        var timer = Stopwatch.StartNew();

        _logger.LogInformation($"Integration with industry successfully - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<IndustryIntegratedEvent>.ShortName);

        await UpdateOrder(context);

        await _confirmPaymentPublisher.Publish(context.Message.OrderId, context.Message.CustomerName);
    }

    private async Task UpdateOrder(ConsumeContext<IndustryIntegratedEvent> context)
    {
        var order = await _orderRepository.GetById(context.Message.OrderId);
        order.UpdateStatus(OrderStatus.IndustryIntegrated);

        var orderStatusHistory = new OrderStatusHistory(order.Id, OrderStatus.IndustryIntegrated);

        await _orderRepository.Update(order);
        await _orderStatusHistoryRepository.Add(orderStatusHistory);
    }
}