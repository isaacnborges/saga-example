using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Saga.Core.Options;

namespace Saga.Core.Extensions;

public static class ConfigurationOptionsExtensions
{
    public static OptionsBuilder<MessageBusOptions> ConfigureMessageBusOptionsExtension(this IServiceCollection services, IConfigurationSection section)
        => services
            .AddOptions<MessageBusOptions>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    
    public static OptionsBuilder<MassTransitHostOptions> ConfigureMassTransitHostOptionsExtensions(this IServiceCollection services, IConfigurationSection section)
        => services
            .AddOptions<MassTransitHostOptions>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();
}