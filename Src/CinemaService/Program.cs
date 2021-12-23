using CinemaService.Api;
using CinemaService.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
var configuration = builder.Configuration;

services.AddGrpc();
services.AddDbContext<CinemaContext>(options => options.UseNpgsql(configuration["DatabaseURI"]));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<CinemaController>();
app.MapGet("/", () => "Connect with a gRPC Client");

app.Run();