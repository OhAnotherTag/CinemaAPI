using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Session;
using System.IO;
using Movie;
using Room;

namespace APIGateway.Controllers;

[ApiController]
[Route("api")]
public class SessionController : ControllerBase
{
    private readonly string _sessionAddress = GrpcAddresses.Session;
    private readonly string _movieAddress = GrpcAddresses.Movie;
    private readonly string _roomAddress = GrpcAddresses.Room;

    public SessionController()
    {
    }

    [HttpGet("cinemas/{cinemaId}/rooms/{roomId}/sessions/{sessionId}")]
    public async Task<ActionResult> GetByIdSession(string cinemaId, string roomId, string sessionId)
    {
        using var channel = GrpcChannel.ForAddress(_sessionAddress);
        var client = new SessionService.SessionServiceClient(channel);

        var reply = await client.GetByIdSessionAsync(new GetByIdSessionRequest {SessionId = sessionId});
        return Ok(reply);
    }
    
    [HttpGet("cinemas/{cinemaId}/rooms/{roomId}/sessions/")]
    public async Task<ActionResult> GetAllSession()
    {
        using var channel = GrpcChannel.ForAddress(_sessionAddress);
        var client = new SessionService.SessionServiceClient(channel);

        var reply = await client.GetAllSessionAsync(new GetAllSessionRequest());
        return Ok(reply);
    }
    
    [HttpPost("cinemas/{cinemaId}/rooms/{roomId}/sessions/")]
    public async Task<ActionResult> CreateSession(CreateSessionRequest request)
    {
        try
        {
            using var sessionChannel = GrpcChannel.ForAddress(_sessionAddress);
            var sessionClient = new SessionService.SessionServiceClient(sessionChannel);

            var reply = await sessionClient.CreateSessionAsync(request);
            
            return Ok(reply);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("cinemas/{cinemaId}/rooms/{roomId}/sessions")]
    public async Task<ActionResult> UpdateSession(UpdateSessionRequest request)
    {
        using var channel = GrpcChannel.ForAddress(_sessionAddress);
        var client = new SessionService.SessionServiceClient(channel);

        var reply = await client.UpdateSessionAsync(request);
        return Ok(reply);
    }

    [HttpDelete("cinemas/{cinemaId}/rooms/{roomId}/sessions/{sessionId}")]
    public async Task<ActionResult> DeleteSession(string sessionId)
    {
        using var channel = GrpcChannel.ForAddress(_sessionAddress);
        var client = new SessionService.SessionServiceClient(channel);

        var reply = await client.DeleteSessionAsync(new DeleteSessionRequest {SessionId = sessionId});
        return Ok(reply);
    }
}