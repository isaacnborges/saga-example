using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Saga.Core.Extensions;
public static class MassTransitExtensions
{
    public static void AddMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                //cfg.OverrideDefaultBusEndpointQueueName

                cfg.Host(configuration.GetConnectionString("RabbitMq"));

                cfg.ConfigureEndpoints(ctx, new KebabCaseEndpointNameFormatter("saga-example", false));
                cfg.UseMessageRetry(retry => { retry.Interval(3, TimeSpan.FromSeconds(5)); });
            });
        });
    }
}
