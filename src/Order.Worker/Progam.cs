using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Order.Domain.Infra.ApiServices;
using Order.Domain.Infra.Repositories;
using Order.Domain.Interfaces;
using Saga.Core;
using Saga.Core.Extensions;
using Serilog;
using System.Reflection;
using Microsoft.Extensions.Options;
using Saga.Core.Options;
using Saga.Core.PipeObservers;

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
            services.AddMongoDbContext(hostContext.Configuration);

            var connectionString = hostContext.Configuration.GetConnectionString("MongoDb");
            var databaseName = "saga-example";

            services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
            services.AddSingleton(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(databaseName));

            services.AddHttpClient<ICartApiService, CartApiService>("Cart", client =>
            {
                client.BaseAddress = new Uri(hostContext.Configuration["Integrations:Cart:BaseUrl"]);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderStatusHistoryRepository, OrderStatusHistoryRepository>();

            services.ConfigureMessageBusOptionsExtension(hostContext.Configuration.GetSection(nameof(MessageBusOptions)));
            services.ConfigureMassTransitHostOptionsExtensions(hostContext.Configuration.GetSection(nameof(MassTransitHostOptions)));
            
            services.AddMassTransit(x =>
            {
                x.AddMongoDbOutbox(o =>
                {
                    o.QueryDelay = TimeSpan.FromSeconds(1);
                    o.ClientFactory(provider => provider.GetRequiredService<IMongoClient>());
                    o.DatabaseFactory(provider => provider.GetRequiredService<IMongoDatabase>());

                    o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);

                    o.UseBusOutbox();
                });

                var entryAssembly = Assembly.GetEntryAssembly();

                x.AddConsumers(entryAssembly);
                x.AddSagaStateMachines(entryAssembly);
                x.AddSagas(entryAssembly);
                x.AddActivities(entryAssembly);

                x.AddDelayedMessageScheduler();
                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((ctx, cfg) =>
                {
                    var options = ctx.GetRequiredService<IOptionsMonitor<MessageBusOptions>>().CurrentValue;
                    
                    cfg.Host(options.ConnectionString);
                    
                    cfg.UseMessageRetry(retry
                        => retry.Incremental(
                            retryLimit: options.RetryLimit,
                            initialInterval: options.InitialInterval,
                            intervalIncrement: options.IntervalIncrement));

                    cfg.ConnectReceiveObserver(new LoggingReceiveObserver());
                    cfg.ConnectConsumeObserver(new LoggingConsumeObserver());
                    cfg.ConnectPublishObserver(new LoggingPublishObserver());
                    cfg.ConnectSendObserver(new LoggingSendObserver());
                    cfg.ConfigureEndpoints(ctx);
                });
            });
            
            services.AddOpenTelemetry(hostContext.Configuration.GetSection(nameof(OpenTelemetrySettings)).Get<OpenTelemetrySettings>());
        })
        .UseSerilog();
