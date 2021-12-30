using Domain;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Session;
using SessionService.Model;

namespace SessionService.Api;

public class SessionController : Session.SessionService.SessionServiceBase
{
    private readonly SessionContext _context;

    public SessionController(SessionContext context)
    {
        _context = context;
    }

    public override async Task<GetByIdSessionReply> GetByIdSession(GetByIdSessionRequest request,
        ServerCallContext context)
    {
        GetByIdSessionReply res;
        try
        {
            var session = await _context.Sessions.SingleAsync(
                r => r.SessionId.ToString() == request.SessionId,
                context.CancellationToken
            );

            res = await Task.FromResult(new GetByIdSessionReply
            {
                SessionId = session.SessionId.ToString(),
                RoomId = session.RoomId.ToString(),
                MovieId = session.MovieId.ToString(),
                StartTime = session.StartingTime.ToString(),
                EndTime = session.EndingTime.ToString(),
                ScreeningDate = session.ScreeningDate.ToString()
            });
        }
        catch (Exception e)
        {
            var httpContext = context.GetHttpContext();
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }

        return res;
    }

    public override async Task<GetAllSessionReply> GetAllSession(GetAllSessionRequest request,
        ServerCallContext context)
    {
        GetAllSessionReply res;
        try
        {
            var sessions = await _context.Sessions.ToListAsync(context.CancellationToken);
            res = await Task.FromResult(new GetAllSessionReply());

            foreach (var session in sessions)
            {
                res.Sessions.Add(new Session.Session
                {
                    SessionId = session.SessionId.ToString(),
                    RoomId = session.RoomId.ToString(),
                    MovieId = session.MovieId.ToString(),
                    StartTime = session.StartingTime.ToString(),
                    EndTime = session.EndingTime.ToString(),
                    ScreeningDate = session.ScreeningDate.ToString()
                });
            }
        }
        catch (Exception e)
        {
            var httpContext = context.GetHttpContext();
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }

        return res;
    }

    public override async Task<CreateSessionReply> CreateSession(CreateSessionRequest request,
        ServerCallContext context)
    {
        const string query = @"
            SELECT *
            FROM session
            WHERE
                room_id = {0}
                AND screening_date = {1}
                AND starting_time BETWEEN {2} AND {3}
                AND ending_time BETWEEN {2} AND {3}
        ";

        try
        {
            var session = new SessionModel
            {
                RoomId = Guid.Parse(request.RoomId),
                MovieId = Guid.Parse(request.MovieId),
                CinemaId = Guid.Parse(request.CinemaId),
                StartingTime = TimeOnly.Parse(request.StartTime),
                EndingTime = TimeOnly.Parse(request.EndTime),
                ScreeningDate = DateOnly.Parse(request.ScreeningDate),
                MovieReleaseDate = DateOnly.Parse(request.MovieReleaseDate),
                MovieRuntime = request.MovieRuntime,
                MovieFormat = request.MovieFormat switch
                {
                    "classic" => Format.Classic,
                    "imax" => Format.Imax,
                    _ => throw new ArgumentException("movie format not supported")
                },
                RoomFormat = request.RoomFormat switch
                {
                    "classic" => Format.Classic,
                    "imax" => Format.Imax,
                    _ => throw new ArgumentException("movie format not supported")
                },
            };

            ValidateSession(session);

            var s = await _context.Sessions.FromSqlRaw(query, session.RoomId, session.ScreeningDate,
                    session.StartingTime, session.EndingTime)
                .FirstOrDefaultAsync(context.CancellationToken);

            if (s is not null)
            {
                throw new ArgumentException("time slot for the new session is already allocated to the room");
            }

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync(context.CancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            var httpContext = context.GetHttpContext();
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }

        return await Task.FromResult(new CreateSessionReply {Message = "new session was created"});
    }

    public override async Task<UpdateSessionReply> UpdateSession(UpdateSessionRequest request,
        ServerCallContext context)
    {
        try
        {
            var session = await _context.Sessions
                .SingleAsync(c => c.SessionId.ToString() == request.SessionId,
                    context.CancellationToken);

            session.RoomId = request.RoomId is null ? session.RoomId : Guid.Parse(request.RoomId);

            session.MovieId = request.MovieId is null ? session.MovieId : Guid.Parse(request.MovieId);

            session.StartingTime = request.StartTime is null
                ? session.StartingTime
                : TimeOnly.Parse(request.StartTime);

            session.EndingTime = request.EndTime is null
                ? session.EndingTime
                : TimeOnly.Parse(request.EndTime);

            session.ScreeningDate = request.ScreeningDate is null
                ? session.ScreeningDate
                : DateOnly.Parse(request.ScreeningDate);

            session.MovieReleaseDate = request.MovieReleaseDate is null
                ? session.ScreeningDate
                : DateOnly.Parse(request.MovieReleaseDate);

            session.MovieRuntime = request.MovieRuntime > 0
                ? session.MovieRuntime
                : request.MovieRuntime;

            session.MovieFormat = request.MovieFormat is null
                ? session.MovieFormat
                : request.MovieFormat switch
                {
                    "classic" => Format.Classic,
                    "imax" => Format.Imax,
                    _ => throw new ArgumentException("movie format not supported")
                };

            session.RoomFormat = request.RoomFormat is null
                ? session.RoomFormat
                : request.RoomFormat switch
                {
                    "classic" => Format.Classic,
                    "imax" => Format.Imax,
                    _ => throw new ArgumentException("movie format not supported")
                };

            ValidateSession(session);

            await _context.SaveChangesAsync(context.CancellationToken);
        }
        catch (Exception e)
        {
            var httpContext = context.GetHttpContext();
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }

        return await Task.FromResult(new UpdateSessionReply {Message = "session was updated"});
    }

    public override async Task<DeleteSessionReply> DeleteSession(DeleteSessionRequest request,
        ServerCallContext context)
    {
        try
        {
            var session = await _context.Sessions
                .SingleAsync(c => c.SessionId.ToString() == request.SessionId,
                    context.CancellationToken);

            _context.Sessions.Remove(session);

            await _context.SaveChangesAsync(context.CancellationToken);
        }
        catch (Exception e)
        {
            var httpContext = context.GetHttpContext();
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }

        return await Task.FromResult(new DeleteSessionReply {Message = "session was delete"});
    }

    private void ValidateSession(SessionModel session)
    {
        if (session.MovieFormat != session.RoomFormat)
        {
            throw new ArgumentException("movie format does not match session's room format");
        }

        if (session.MovieReleaseDate.CompareTo(session.ScreeningDate) > 0)
        {
            throw new ArgumentException("session screening date must be equal or greater than movie's release date");
        }

        var validRuntime = session.StartingTime.AddMinutes(session.MovieRuntime);

        if (session.EndingTime.CompareTo(validRuntime) != 0)
        {
            throw new ArgumentException("session's end time must be equal start time + movie runtime");
        }
    }
}