using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Order.Domain.Interfaces;
using Order.Domain.Models;
using Order.Worker.Interfaces;
using Saga.Contracts;
using System.Diagnostics;

namespace Order.Worker.Consumers;
public class PaymentAuthorizedEventConsumer : IConsumer<PaymentAuthorizedEvent>
{
    private readonly ILogger<PaymentAuthorizedEventConsumer> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderStatusHistoryRepository _orderStatusHistoryRepository;
    private readonly IIntegrateIndustryPublisher _integrateIndustryPublisher;

    public PaymentAuthorizedEventConsumer(
        ILogger<PaymentAuthorizedEventConsumer> logger,
        IOrderRepository orderRepository,
        IOrderStatusHistoryRepository orderStatusHistoryRepository,
        IIntegrateIndustryPublisher integrateIndustryPublisher)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _orderStatusHistoryRepository = orderStatusHistoryRepository;
        _integrateIndustryPublisher = integrateIndustryPublisher;
    }

    public async Task Consume(ConsumeContext<PaymentAuthorizedEvent> context)
    {
        var timer = Stopwatch.StartNew();

        _logger.LogInformation($"Payment authorized - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<PaymentAuthorizedEvent>.ShortName);

        await UpdateOrder(context);

        await _integrateIndustryPublisher.Publish(context.Message.OrderId, context.Message.CustomerName);
    }

    private async Task UpdateOrder(ConsumeContext<PaymentAuthorizedEvent> context)
    {
        var order = await _orderRepository.GetById(context.Message.OrderId);
        order.UpdateStatus(OrderStatus.PaymentAuthorized);

        var orderStatusHistory = new OrderStatusHistory(order.Id, OrderStatus.PaymentAuthorized);

        await _orderRepository.Update(order);
        await _orderStatusHistoryRepository.Add(orderStatusHistory);
    }
}