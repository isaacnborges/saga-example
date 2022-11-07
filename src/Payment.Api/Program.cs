using Saga.Core;
using Saga.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddSerilog("Payment Api");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var settings = builder.Configuration.GetSection(nameof(OpenTelemetrySettings)).Get<OpenTelemetrySettings>();
builder.Services.AddOpenTelemetry(settings);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
