using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Order.Domain.Infra.Repositories;
using Order.Domain.Interfaces;
using Order.Domain.Models;
using Saga.Contracts;
using System.Diagnostics;

namespace Order.Worker.Consumers;
public class PaymentAuthorizedEventConsumer : IConsumer<PaymentAuthorizedEvent>
{
    private readonly IBus _bus;
    private readonly ILogger<PaymentAuthorizedEventConsumer> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderStatusHistoryRepository _orderStatusHistoryRepository;

    public PaymentAuthorizedEventConsumer(
        IBus bus, 
        ILogger<PaymentAuthorizedEventConsumer> logger, 
        IOrderRepository orderRepository, 
        IOrderStatusHistoryRepository orderStatusHistoryRepository)
    {
        _bus = bus;
        _logger = logger;
        _orderRepository = orderRepository;
        _orderStatusHistoryRepository = orderStatusHistoryRepository;
    }

    public async Task Consume(ConsumeContext<PaymentAuthorizedEvent> context)
    {
        var timer = Stopwatch.StartNew();

        _logger.LogInformation($"Payment authorized - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<PaymentAuthorizedEvent>.ShortName);

        var order = await _orderRepository.GetById(context.Message.OrderId);
        order.UpdateStatus(OrderStatus.PaymentAuthorized);

        var orderStatusHistory = new OrderStatusHistory(order.Id, OrderStatus.PaymentAuthorized);

        await _orderRepository.Update(order);
        await _orderStatusHistoryRepository.Add(orderStatusHistory);

        var command = new IntegrateIndustryCommand(context.Message.OrderId, context.Message.CustomerName, InVar.Timestamp);
        await _bus.Publish(command);
        _logger.LogInformation($"Send IntegrateIndustryCommand - OrderId: {command.OrderId}");
    }
}