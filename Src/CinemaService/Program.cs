using CinemaService.Api;
using CinemaService.Messaging.Senders;
using CinemaService.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;

var dbUri = Environment.GetEnvironmentVariable("DB_URI") ?? throw new ArgumentException("DB_URI cannot be null"); 

services.AddGrpc();
services.AddDbContext<CinemaContext>(options => options.UseNpgsql(dbUri));

services.AddSingleton<IDeleteCascadeCinemaSender, DeleteCascadeCinemaSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<CinemaController>();
app.MapGet("/", () => "Connect with a gRPC Client");

app.Run();