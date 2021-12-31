// using SessionService.Services;

using MassTransit;
using Microsoft.EntityFrameworkCore;
using SessionService.Api;
using SessionService.Messaging.Consumers;
using SessionService.Messaging.Receiver;
using SessionService.Model;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
var services = builder.Services;

var dbUri = Environment.GetEnvironmentVariable("DB_URI") ?? throw new ArgumentException("DB_URI cannot be null");
var mqHost = Environment.GetEnvironmentVariable("MQ_HOST") ?? throw new ArgumentException("MQ_HOST cannot be null");
var mqUser = Environment.GetEnvironmentVariable("MQ_USERNAME") ?? throw new ArgumentException("MQ_USERNAME cannot be null");
var mqPassword = Environment.GetEnvironmentVariable("MQ_PASSWORD") ?? throw new ArgumentException("MQ_PASSWORD cannot be null");

// Add services to the container.
services.AddGrpc();

services.AddDbContext<SessionContext>(
    options => options.UseNpgsql(dbUri),
    ServiceLifetime.Transient,
    ServiceLifetime.Transient);

services.AddMassTransit(config =>
{
    config.AddConsumer<DeleteCinemaCascadeConsumer>();
    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(mqHost, "/", h =>
        {
            h.Username(mqUser);
            h.Password(mqPassword);
        });
        cfg.ReceiveEndpoint("session-service", e =>
        {
            e.ConfigureConsumer<DeleteCinemaCascadeConsumer>(context);
        });
    });
});

services.AddMassTransitHostedService();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<SessionController>();
app.MapGet("/", () => "");
app.Run();