using MassTransit;
using Microsoft.EntityFrameworkCore;
using RoomService.Api;
using RoomService.Messaging.Consumers;
using RoomService.Messaging.Receivers;
using RoomService.Messaging.Sender;
using RoomService.Model;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

var dbUri = Environment.GetEnvironmentVariable("DB_URI") ?? throw new ArgumentException("DB_URI cannot be null"); // Add services to the container.
var mqHost = Environment.GetEnvironmentVariable("MQ_HOST") ?? throw new ArgumentException("MQ_HOST cannot be null");
var mqUser = Environment.GetEnvironmentVariable("MQ_USERNAME") ?? throw new ArgumentException("MQ_USERNAME cannot be null");
var mqPassword = Environment.GetEnvironmentVariable("MQ_PASSWORD") ?? throw new ArgumentException("MQ_PASSWORD cannot be null");

services.AddGrpc();

services.AddTransient<IDeleteCascadeRoomSender, DeleteCascadeRoomSender>();

services.AddDbContext<RoomContext>(
    option => option.UseNpgsql(dbUri),
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
        
        cfg.ReceiveEndpoint("room-service", e =>
        {
            e.ConfigureConsumer<DeleteCinemaCascadeConsumer>(context);
        });
    });
});

services.AddMassTransitHostedService();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<RoomController>();
app.MapGet("/",
    () => "");

app.Run();