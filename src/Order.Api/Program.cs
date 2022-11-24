using MassTransit;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Order.Domain.Infra.ApiServices;
using Order.Domain.Infra.Repositories;
using Order.Domain.Interfaces;
using Order.Domain.Saga;
using Order.Domain.Services;
using Saga.Core;
using Saga.Core.Extensions;
using Saga.Core.Options;

var builder = WebApplication.CreateBuilder(args);
builder.AddSerilog("Order Api");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var settings = builder.Configuration.GetSection(nameof(OpenTelemetrySettings)).Get<OpenTelemetrySettings>();
builder.Services.AddOpenTelemetry(settings);

builder.Services.AddHttpClient<ICartApiService, CartApiService>("Cart", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Integrations:Cart:BaseUrl"]);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<IPaymentApiService, PaymentApiService>("Payment", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Integrations:Payment:BaseUrl"]);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddMongoDbContext(builder.Configuration);

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderStatusHistoryRepository, OrderStatusHistoryRepository>();

var connectionString = builder.Configuration.GetConnectionString("MongoDb");
var databaseName = "saga-example";

builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
builder.Services.AddSingleton(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(databaseName));

builder.Services.ConfigureMessageBusOptionsExtension(builder.Configuration.GetSection(nameof(MessageBusOptions)));
builder.Services.ConfigureMassTransitHostOptionsExtensions(builder.Configuration.GetSection(nameof(MassTransitHostOptions)));

builder.Services.AddMassTransit(x =>
{
    x.AddMongoDbOutbox(o =>
    {
        o.DisableInboxCleanupService();
        o.ClientFactory(provider => provider.GetRequiredService<IMongoClient>());
        o.DatabaseFactory(provider => provider.GetRequiredService<IMongoDatabase>());

        o.UseBusOutbox(bo => bo.DisableDeliveryService());
    });

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
        cfg.ConnectSendObserver(new LoggingSendObserver());
        
        cfg.ConfigureEndpoints(ctx);
    });

    x.AddSagaStateMachine<OrderStateMachine, OrderState, OrderStateMachineDefinition>()
        .MongoDbRepository(r =>
        {
            r.ClientFactory(provider => provider.GetRequiredService<IMongoClient>());
            r.DatabaseFactory(provider => provider.GetRequiredService<IMongoDatabase>());
        });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
