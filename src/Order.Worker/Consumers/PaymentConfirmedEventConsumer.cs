using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Order.Domain.Interfaces;
using Order.Domain.Models;
using Saga.Contracts;
using System.Diagnostics;

namespace Order.Worker.Consumers;
public class PaymentConfirmedEventConsumer : IConsumer<PaymentConfirmedEvent>
{
    private readonly IBus _bus;
    private readonly ILogger<PaymentConfirmedEventConsumer> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderStatusHistoryRepository _orderStatusHistoryRepository;

    public PaymentConfirmedEventConsumer(
        IBus bus,
        ILogger<PaymentConfirmedEventConsumer> logger,
        IOrderRepository orderRepository,
        IOrderStatusHistoryRepository orderStatusHistoryRepository)
    {
        _bus = bus;
        _logger = logger;
        _orderRepository = orderRepository;
        _orderStatusHistoryRepository = orderStatusHistoryRepository;
    }

    public async Task Consume(ConsumeContext<PaymentConfirmedEvent> context)
    {
        var timer = Stopwatch.StartNew();

        await Task.Delay(TimeSpan.FromSeconds(5));

        _logger.LogInformation($"Payment confirmed - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<PaymentConfirmedEvent>.ShortName);

        var order = await _orderRepository.GetById(context.Message.OrderId);
        order.UpdateStatus(OrderStatus.PaymentConfirmed);

        var orderStatusHistory = new OrderStatusHistory(order.Id, OrderStatus.PaymentConfirmed);

        await _orderRepository.Update(order);
        await _orderStatusHistoryRepository.Add(orderStatusHistory);

        var command = new OrderProcessedEvent(context.Message.OrderId, context.Message.CustomerName, InVar.Timestamp);
        await _bus.Publish(command);
        _logger.LogInformation($"Send OrderProcessedEvent - OrderId: {command.OrderId}");
    }
}