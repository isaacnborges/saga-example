using MassTransit;
using Microsoft.Extensions.Logging;
using Order.Worker.Interfaces;
using Saga.Contracts;

namespace Order.Worker.Publishers;
public class IntegrateIndustryPublisher : IIntegrateIndustryPublisher
{
    private readonly IBus _bus;
    private readonly ILogger<IntegrateIndustryPublisher> _logger;

    public IntegrateIndustryPublisher(IBus bus, ILogger<IntegrateIndustryPublisher> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task Publish(Guid orderId, string customerName)
    {
        var command = new IntegrateIndustryCommand(orderId, customerName, InVar.Timestamp);
        await _bus.Publish(command);
        _logger.LogInformation($"Send IntegrateIndustryCommand - OrderId: {command.OrderId}");
    }
}
