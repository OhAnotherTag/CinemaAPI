// using SessionService.Services;

using Microsoft.EntityFrameworkCore;
using SessionService.Api;
using SessionService.Messaging.Receiver;
using SessionService.Model;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
var services = builder.Services;
var configuration = builder.Configuration;
// Add services to the container.
services.AddGrpc();

services.AddDbContext<SessionContext>(
    options => options.UseNpgsql(configuration["DatabaseURI"]),
    ServiceLifetime.Transient,
    ServiceLifetime.Transient);

services.AddHostedService<DeleteCascadeRoomReceiver>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<SessionController>();
app.MapGet("/", () => "");
app.Run();