using MassTransit;
using Order.Api.ApiServices;
using Order.Api.Interfaces;
using Order.Domain.Saga;
using Order.Domain.Services;
using Saga.Contracts;
using Saga.Core;
using Saga.Core.Extensions;
using Serilog;


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

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));

        cfg.ConfigureEndpoints(ctx);
    });

    x.AddSagaStateMachine<OrderStateMachine, OrderState>(typeof(OrderStateMachineDefinition))
        .MongoDbRepository(r =>
        {
            r.Connection = "mongodb://mongo:mongo@localhost:27017";
            r.DatabaseName = "saga-orders";
        });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
