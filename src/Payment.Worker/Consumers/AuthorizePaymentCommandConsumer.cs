using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using Saga.Contracts;
using System.Diagnostics;

namespace Payment.Worker.Consumers;
public class AuthorizePaymentCommandConsumer : IConsumer<AuthorizePaymentCommand>
{
    private readonly IBus _bus;
    private readonly ILogger<AuthorizePaymentCommandConsumer> _logger;

    public AuthorizePaymentCommandConsumer(IBus bus, ILogger<AuthorizePaymentCommandConsumer> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<AuthorizePaymentCommand> context)
    {
        var timer = Stopwatch.StartNew();

        _logger.LogInformation($"Payment authorized - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<PaymentAuthorizedEvent>.ShortName);

        var paymentAuthorizedEvent = new PaymentAuthorizedEvent(context.Message.OrderId, context.Message.CustomerName);
        await _bus.Publish(paymentAuthorizedEvent);
        _logger.LogInformation($"Send PaymentAuthorizedEvent - OrderId: {paymentAuthorizedEvent.OrderId}");
    }
}

public class PaymentAuthorizedConsumerDefinition : ConsumerDefinition<AuthorizePaymentCommandConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<AuthorizePaymentCommandConsumer> consumerConfigurator)
    {
        consumerConfigurator.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(3)));
    }
}
