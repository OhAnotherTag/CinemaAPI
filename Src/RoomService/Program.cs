using MassTransit;
using Microsoft.EntityFrameworkCore;
using RoomService.Api;
using RoomService.Messaging.Consumers;
using RoomService.Messaging.Receivers;
using RoomService.Messaging.Sender;
using RoomService.Model;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var dbUri = Environment.GetEnvironmentVariable("DB_URI") ??
            throw new ArgumentException("DB_URI cannot be null"); // Add services to the container.
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