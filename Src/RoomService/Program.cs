using Microsoft.EntityFrameworkCore;
using RoomService.Api;
using RoomService.Model;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;
// Add services to the container.
services.AddGrpc();
services.AddDbContext<RoomContext>(option => option.UseNpgsql(configuration["DatabaseURI"]));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<RoomController>();
app.MapGet("/",
    () => "");

app.Run();