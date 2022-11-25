using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Payment.Worker.Interfaces;
using Saga.Contracts;
using System.Diagnostics;

namespace Payment.Worker.Consumers;
public class AuthorizePaymentCommandConsumer : IConsumer<AuthorizePaymentCommand>
{
    private readonly ILogger<AuthorizePaymentCommandConsumer> _logger;
    private readonly IPaymentAuthorizedPublisher _paymentAuthorizedPublisher;

    public AuthorizePaymentCommandConsumer(ILogger<AuthorizePaymentCommandConsumer> logger, IPaymentAuthorizedPublisher paymentAuthorizedPublisher)
    {
        _logger = logger;
        _paymentAuthorizedPublisher = paymentAuthorizedPublisher;
    }

    public async Task Consume(ConsumeContext<AuthorizePaymentCommand> context)
    {
        var timer = Stopwatch.StartNew();

        await Task.Delay(TimeSpan.FromSeconds(5));

        _logger.LogInformation($"Payment authorized - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<AuthorizePaymentCommand>.ShortName);

        await _paymentAuthorizedPublisher.Publish(context.Message.OrderId, context.Message.CustomerName);
    }
}