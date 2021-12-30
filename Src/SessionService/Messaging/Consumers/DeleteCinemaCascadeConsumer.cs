using Domain.Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SessionService.Model;

namespace SessionService.Messaging.Consumers;

public class DeleteCinemaCascadeConsumer : IConsumer<CinemaDeleted>
{
    private readonly ILogger<DeleteCinemaCascadeConsumer> _logger;
    private readonly SessionContext _context;

    public DeleteCinemaCascadeConsumer(ILogger<DeleteCinemaCascadeConsumer> logger, SessionContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task Consume(ConsumeContext<CinemaDeleted> context)
    {
        _logger.LogInformation("Event DeleteCinemaCascade was evoked");
        
        var room = await _context.Sessions
            .SingleOrDefaultAsync(r => r.CinemaId.ToString() == context.Message.CinemaId, context.CancellationToken);
        
        if (room is null)
        {
            _logger.LogInformation("Session not found");
            return;
        }
        
        _context.Sessions.Remove(room);
        
        await _context.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("Deleted Session with CinemaId: {CinemaId}", context.Message.CinemaId);
    }
}