using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Order.Domain.Infra.ApiServices;
using Order.Domain.Interfaces;
using Saga.Core;
using Saga.Core.Extensions;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.AddSerilog("Order Worker");
Log.Information("Starting Order Worker");

var host = CreateHostBuilder(args).Build();

await host.RunAsync();


static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureHostConfiguration(config => config.AddEnvironmentVariables())
        .ConfigureServices((hostContext, services) =>
        {
            services.AddHttpClient<ICartApiService, CartApiService>("Cart", client =>
            {
                client.BaseAddress = new Uri(hostContext.Configuration["Integrations:Cart:BaseUrl"]);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            var settings = hostContext.Configuration.GetSection(nameof(OpenTelemetrySettings)).Get<OpenTelemetrySettings>();

            services.AddMassTransit(x =>
            {
                var entryAssembly = Assembly.GetEntryAssembly();

                x.AddConsumers(entryAssembly);
                x.AddSagaStateMachines(entryAssembly);
                x.AddSagas(entryAssembly);
                x.AddActivities(entryAssembly);

                x.AddDelayedMessageScheduler();
                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(hostContext.Configuration.GetConnectionString("RabbitMq"));

                    cfg.ConfigureEndpoints(ctx);
                });
            });

            services.AddOptions<MassTransitHostOptions>().Configure(options =>
            {
                options.WaitUntilStarted = true;
            });

            services.AddOpenTelemetry(settings);
        })
        .UseSerilog();
