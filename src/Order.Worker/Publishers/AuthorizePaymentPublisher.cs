using MassTransit;
using Microsoft.Extensions.Logging;
using Order.Worker.Interfaces;
using Saga.Contracts;

namespace Order.Worker.Publishers;
public class AuthorizePaymentPublisher : IAuthorizePaymentPublisher
{
    private readonly ILogger<AuthorizePaymentPublisher> _logger;
    private readonly IBus _bus;

    public AuthorizePaymentPublisher(ILogger<AuthorizePaymentPublisher> logger, IBus bus)
    {
        _logger = logger;
        _bus = bus;
    }

    public async Task Publish(Guid orderId, string customerName)
    {
        var command = new AuthorizePaymentCommand(orderId, customerName, InVar.Timestamp);
        await _bus.Publish(command);
        _logger.LogInformation($"Send AuthorizePaymentCommand - OrderId: {command.OrderId}");
    }
}
