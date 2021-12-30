using Domain.Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using RoomService.Model;

namespace RoomService.Messaging.Consumers;

public class DeleteCinemaCascadeConsumer : IConsumer<CinemaDeleted>
{
    private readonly ILogger<DeleteCinemaCascadeConsumer> _logger;
    private readonly RoomContext _context;

    public DeleteCinemaCascadeConsumer(ILogger<DeleteCinemaCascadeConsumer> logger, RoomContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task Consume(ConsumeContext<CinemaDeleted> context)
    {
        _logger.LogInformation("Event DeleteCinemaCascade was evoked");
        
        var room = await _context.Rooms
            .SingleOrDefaultAsync(r => r.CinemaId.ToString() == context.Message.CinemaId, context.CancellationToken);
        if (room is null)
        {
            _logger.LogInformation("Room not found");
            return;
        }
        _context.Rooms.Remove(room);
        
        await _context.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("Deleted Room with CinemaId: {CinemaId}", context.Message.CinemaId);
    }
}