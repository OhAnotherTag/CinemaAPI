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
            ValidateSession(request);
            
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
    
    private async void ValidateSession(CreateSessionRequest session)
    {
        using var roomChannel = GrpcChannel.ForAddress(_roomAddress);
        var roomClient = new RoomService.RoomServiceClient(roomChannel);
        var roomReply = roomClient.GetByIdRoomAsync(new GetByIdRoomRequest{RoomId = session.RoomId}).ResponseAsync;

        using var movieChannel = GrpcChannel.ForAddress(_movieAddress);
        var movieClient = new MovieService.MovieServiceClient(movieChannel);
        var movieReply = movieClient.GetByIdMovieAsync(new GetByIdMovieRequest{MovieId = session.MovieId}).ResponseAsync;

        await Task.WhenAll(roomReply, movieReply);
            
        var movie = await movieReply;
        var room = await roomReply;
        
        if (room.Format != movie.Format)
        {
            throw new ArgumentException("movie format does not match session's room format");
        }

        var movieDate = DateOnly.Parse($"{movie.ReleaseYear}-{movie.ReleaseMonth}-{movie.ReleaseDay}");
        var sessionDate = DateOnly.Parse($"{session.ScreeningYear}-{session.ScreeningMonth}-{session.ScreeningDay}");

        if (movieDate.CompareTo(sessionDate) > 0)
        {
            throw new ArgumentException("session screening date must be equal or greater than movie's release date");
        }

        var startTime = TimeOnly.Parse($"{session.StartTimeHour}:{session.StartTimeMinute}:00");
        var endTime = TimeOnly.Parse($"{session.EndTimeHour}:{session.EndTimeMinute}:00");

        var validRuntime = startTime.AddMinutes(movie.Runtime);

        if (endTime.CompareTo(validRuntime) != 0)
        {
            throw new ArgumentException("session's end time must be equal start time + movie runtime");
        }
    }
    
    [HttpPut("cinemas/{cinemaId}/rooms/{roomId}/sessions")]
    public async Task<ActionResult> UpdateCinema(UpdateSessionRequest request)
    {
        var req = new CreateSessionRequest
        {
            RoomId = request.RoomId,
            MovieId = request.MovieId,
            StartTimeHour = request.StartTimeHour,
            StartTimeMinute = request.StartTimeMinute,
            EndTimeHour = request.EndTimeHour,
            EndTimeMinute = request.EndTimeMinute,
            ScreeningDay = request.ScreeningDay,
            ScreeningMonth = request.ScreeningMonth,
            ScreeningYear = request.ScreeningYear,
        };
        
        ValidateSession(req);
        
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