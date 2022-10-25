using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Saga.Core.Extensions;
public static class OpenTelemetryExtensions
{
    public static void AddOpenTelemetry(this IServiceCollection services, OpenTelemetrySettings settings)
    {
        services.AddOpenTelemetryTracing(traceProvider =>
        {
            traceProvider.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(settings.ServiceName)
                    .AddTelemetrySdk()
                    .AddEnvironmentVariableDetector())
                .AddSource("MassTransit")
                .AddAspNetCoreInstrumentation()
                .AddJaegerExporter(o =>
                {
                    o.AgentHost = settings.AgentHost;
                    o.AgentPort = settings.AgentPort;
                });
        });
    }
}
