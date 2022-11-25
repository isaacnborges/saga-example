using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Order.Domain.Interfaces;
using Order.Domain.Models;
using Saga.Contracts;
using System.Diagnostics;

namespace Order.Worker.Consumers;
public class IndustryFailedEventConsumer : IConsumer<IndustryFailedEvent>
{
    private readonly ICartApiService _cartApiService;
    private readonly ILogger<IndustryFailedEventConsumer> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderStatusHistoryRepository _orderStatusHistoryRepository;

    public IndustryFailedEventConsumer(
        ICartApiService cartApiService, 
        ILogger<IndustryFailedEventConsumer> logger, 
        IOrderRepository orderRepository, 
        IOrderStatusHistoryRepository orderStatusHistoryRepository)
    {
        _cartApiService = cartApiService;
        _logger = logger;
        _orderRepository = orderRepository;
        _orderStatusHistoryRepository = orderStatusHistoryRepository;
    }

    public async Task Consume(ConsumeContext<IndustryFailedEvent> context)
    {
        var timer = Stopwatch.StartNew();

        _logger.LogInformation($"Integration with industry failed - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<IndustryFailedEvent>.ShortName);

        await UpdateOrder(context);

        await ReopenCart();
    }

    private async Task UpdateOrder(ConsumeContext<IndustryFailedEvent> context)
    {
        var order = await _orderRepository.GetById(context.Message.OrderId);
        order.UpdateStatus(OrderStatus.IndustryFailed);

        var orderStatusHistory = new OrderStatusHistory(order.Id, OrderStatus.IndustryFailed);

        await _orderRepository.Update(order);
        await _orderStatusHistoryRepository.Add(orderStatusHistory);
    }

    private async Task ReopenCart()
    {
        _logger.LogInformation("Rollback to cart api");
        var cartResponse = await _cartApiService.ReopenCart();
        _logger.LogInformation($"Carrinho reaberto - {cartResponse}");
    }
}