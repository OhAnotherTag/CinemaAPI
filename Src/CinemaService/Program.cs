using CinemaService.Api;
using CinemaService.Messaging.Senders;
using CinemaService.Model;
using Domain.Contracts;
using Domain.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;

var dbUri = Environment.GetEnvironmentVariable("DB_URI") ?? throw new ArgumentException("DB_URI cannot be null");

services.AddGrpc();

services.AddDbContext<CinemaContext>(options => options.UseNpgsql(dbUri), ServiceLifetime.Transient, ServiceLifetime.Transient);

services.AddMassTransit(config => { config.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context)); });
services.AddMassTransitHostedService();

services.AddTransient(typeof(ISender<CinemaDeleted>), typeof(DeleteCascadeCinemaSender));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<CinemaController>();
app.MapGet("/", () => "Connect with a gRPC Client");

app.Run();