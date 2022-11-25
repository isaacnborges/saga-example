using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Logging;
using OrderVendor.Worker.Publishers.Interfaces;
using Saga.Contracts;
using System.Diagnostics;

namespace OrderVendor.Worker.Consumers;
public class IntegrateIndustryCommandConsumer : IConsumer<IntegrateIndustryCommand>
{
    private readonly ILogger<IntegrateIndustryCommandConsumer> _logger;
    private readonly IIndustryIntegratedPublisher _industryIntegratedPublisher;
    private readonly IIndustryFailedPublisher _industryFailedPublisher;

    public IntegrateIndustryCommandConsumer(
        ILogger<IntegrateIndustryCommandConsumer> logger, 
        IIndustryIntegratedPublisher industryIntegratedPublisher, 
        IIndustryFailedPublisher industryFailedPublisher)
    {
        _logger = logger;
        _industryIntegratedPublisher = industryIntegratedPublisher;
        _industryFailedPublisher = industryFailedPublisher;
    }

    public async Task Consume(ConsumeContext<IntegrateIndustryCommand> context)
    {
        var timer = Stopwatch.StartNew();

        await Task.Delay(TimeSpan.FromSeconds(15));

        _logger.LogInformation($"Industry integreated - OrderId: {context.Message.OrderId}");

        await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<IntegrateIndustryCommand>.ShortName);


        if (SimulateFailedIntegrationWithIndustry())
        {
            await _industryFailedPublisher.Publish(context.Message.OrderId, context.Message.CustomerName);
            return;
        }


        await _industryIntegratedPublisher.Publish(context.Message.OrderId, context.Message.CustomerName);
    }

    private static bool SimulateFailedIntegrationWithIndustry()
    {
        return new Random().Next(1, 4) % 2 == 0;
    }
}
