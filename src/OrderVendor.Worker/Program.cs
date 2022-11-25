using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Saga.Core;
using Saga.Core.Extensions;
using Serilog;
using System.Reflection;
using Saga.Core.PipeObservers;
using OrderVendor.Worker.Publishers;
using OrderVendor.Worker.Publishers.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.AddSerilog("Order-Vendor Worker");
Log.Information("Starting Order-Vendor Worker");

var host = CreateHostBuilder(args).Build();

await host.RunAsync();


static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureHostConfiguration(config => config.AddEnvironmentVariables())
        .ConfigureServices((hostContext, services) =>
        {
            var connectionString = hostContext.Configuration.GetConnectionString("MongoDb");
            var databaseName = "saga-example";

            services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
            services.AddSingleton(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(databaseName));

            services.AddScoped<IIndustryFailedPublisher, IndustryFailedPublisher>();
            services.AddScoped<IIndustryFailedPublisher, IndustryFailedPublisher>();

            var settings = hostContext.Configuration.GetSection(nameof(OpenTelemetrySettings)).Get<OpenTelemetrySettings>();

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
                    cfg.Host(hostContext.Configuration.GetConnectionString("RabbitMq"));

                    cfg.ConnectReceiveObserver(new LoggingReceiveObserver());
                    cfg.ConnectConsumeObserver(new LoggingConsumeObserver());
                    cfg.ConnectPublishObserver(new LoggingPublishObserver());
                    cfg.ConnectSendObserver(new LoggingSendObserver());
                    cfg.ConfigureEndpoints(ctx);
                    cfg.ConfigureEndpoints(ctx, new KebabCaseEndpointNameFormatter("saga-example", false));
                });
            });

            services.AddOptions<MassTransitHostOptions>().Configure(options =>
            {
                options.WaitUntilStarted = true;
            });

            services.AddOpenTelemetry(settings);
        })
        .UseSerilog();