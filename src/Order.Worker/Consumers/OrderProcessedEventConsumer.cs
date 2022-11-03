using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Order.Domain.Interfaces;
using Order.Domain.Models;
using Saga.Contracts;
using System.Diagnostics;

namespace Order.Worker.Consumers;
public class OrderProcessedEventConsumer : IConsumer<OrderProcessedEvent>
{
    private readonly ILogger<OrderProcessedEventConsumer> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderStatusHistoryRepository _orderStatusHistoryRepository;

    public OrderProcessedEventConsumer(
        ILogger<OrderProcessedEventConsumer> logger, 
        IOrderRepository orderRepository, 
        IOrderStatusHistoryRepository orderStatusHistoryRepository)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _orderStatusHistoryRepository = orderStatusHistoryRepository;
    }

    public async Task Consume(ConsumeContext<OrderProcessedEvent> context)
    {
        var timer = Stopwatch.StartNew();

        await Task.Delay(TimeSpan.FromSeconds(6));

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<PaymentAuthorizedEvent>.ShortName);

        var order = await _orderRepository.GetById(context.Message.OrderId);
        order.UpdateStatus(OrderStatus.Finalized);

        var orderStatusHistory = new OrderStatusHistory(order.Id, OrderStatus.Finalized);

        await _orderRepository.Update(order);
        await _orderStatusHistoryRepository.Add(orderStatusHistory);

        _logger.LogInformation($"Order processed successfully - OrderId: {context.Message.OrderId}");
    }
}
