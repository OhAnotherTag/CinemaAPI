// using MovieService.Services;

using Microsoft.EntityFrameworkCore;
using MovieService.Api;
using MovieService.Model;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

var service = builder.Services;
var dbUri = Environment.GetEnvironmentVariable("DB_URI") ?? throw new ArgumentException("DB_URI cannot be null");

// Add services to the container.

service.AddGrpc();
service.AddDbContext<MovieContext>(options => options.UseNpgsql(dbUri));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<MovieController>();
app.MapGet("/",
    () => "");
app.Run();