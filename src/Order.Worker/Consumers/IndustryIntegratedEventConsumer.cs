using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Order.Domain.Interfaces;
using Order.Domain.Models;
using Saga.Contracts;
using System.Diagnostics;

namespace Order.Worker.Consumers;
public class IndustryIntegratedEventConsumer : IConsumer<IndustryIntegratedEvent>
{
    private readonly IBus _bus;
    private readonly ILogger<IndustryIntegratedEventConsumer> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderStatusHistoryRepository _orderStatusHistoryRepository;

    public IndustryIntegratedEventConsumer(
        IBus bus,
        ILogger<IndustryIntegratedEventConsumer> logger,
        IOrderRepository orderRepository,
        IOrderStatusHistoryRepository orderStatusHistoryRepository)
    {
        _bus = bus;
        _logger = logger;
        _orderRepository = orderRepository;
        _orderStatusHistoryRepository = orderStatusHistoryRepository;
    }

    public async Task Consume(ConsumeContext<IndustryIntegratedEvent> context)
    {
        var timer = Stopwatch.StartNew();

        _logger.LogInformation($"Integration with industry successfully - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<IndustryIntegratedEvent>.ShortName);

        var order = await _orderRepository.GetById(context.Message.OrderId);
        order.UpdateStatus(OrderStatus.IndustryIntegrated);

        var orderStatusHistory = new OrderStatusHistory(order.Id, OrderStatus.IndustryIntegrated);

        await _orderRepository.Update(order);
        await _orderStatusHistoryRepository.Add(orderStatusHistory);

        var command = new ConfirmPaymentCommand(context.Message.OrderId, context.Message.CustomerName, InVar.Timestamp);
        await _bus.Publish(command);
        _logger.LogInformation($"Send ConfirmPaymentCommand - OrderId: {command.OrderId}");
    }
}