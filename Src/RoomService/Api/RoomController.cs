using Domain;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Room;
using RoomService.Model;

namespace RoomService.Api;

public class RoomController : Room.RoomService.RoomServiceBase
{
    private readonly RoomContext _context;

    public RoomController(RoomContext context)
    {
        _context = context;
    }

    public override async Task<GetByIdRoomReply> GetByIdRoom(GetByIdRoomRequest request, ServerCallContext context)
    {
        GetByIdRoomReply res;
        try
        {
            var room = await _context.Rooms.SingleAsync(
                r => r.RoomId.ToString() == request.RoomId,
                context.CancellationToken
            );

            res = await Task.FromResult(new GetByIdRoomReply
            {
                RoomId = room.RoomId.ToString(),
                Seats = room.Seats,
                Number = room.Number,
                CinemaId = room.CinemaId.ToString(),
                Format = room.Format.ToString().ToLower()
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

    public override async Task<GetAllRoomReply> GetAllRoom(GetAllRoomRequest request, ServerCallContext context)
    {
        GetAllRoomReply res;
        try
        {
            var rooms = await _context.Rooms.ToListAsync(context.CancellationToken);
            res = await Task.FromResult(new GetAllRoomReply());

            foreach (var room in rooms)
            {
                res.Rooms.Add(new Room.Room
                {
                    RoomId = room.RoomId.ToString(),
                    Seats = room.Seats,
                    Number = room.Number,
                    CinemaId = room.CinemaId.ToString(),
                    Format = room.Format.ToString().ToLower()
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

    public override async Task<CreateRoomReply> CreateRoom(CreateRoomRequest request, ServerCallContext context)
    {
        try
        {
            var room = new RoomModel
            {
                CinemaId = Guid.Parse(request.CinemaId),
                Seats = request.Seats,
                Number = request.Number,
                Format = request.Format switch
                {
                    "classic" => Format.Classic,
                    "imax" => Format.Imax,
                    _ => throw new ArgumentException("movie format not supported")
                }
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync(context.CancellationToken);
        }
        catch (Exception e)
        {
            var httpContext = context.GetHttpContext();
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            
            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }
        
        return await Task.FromResult(new CreateRoomReply{Message = "new room was created"});
    }

    public override async Task<UpdateRoomReply> UpdateRoom(UpdateRoomRequest request, ServerCallContext context)
    {
        try
        {
            var room = await _context.Rooms
                .SingleAsync(r => r.RoomId.ToString() == request.RoomId,
                    context.CancellationToken);

            room.CinemaId = request.CinemaId is null ? room.CinemaId : Guid.Parse(request.CinemaId);
            
            room.Seats = request.Seats <= 0 ? room.Seats : request.Seats;
            
            room.Number = request.Number <= 0 ? room.Number : request.Number;
            
            room.Format = request.Format is null
                ? room.Format
                : request.Format switch
                {
                    "classic" => Format.Classic,
                    "imax" => Format.Imax,
                    _ => throw new ArgumentException("movie format not supported")
                };
            
            await _context.SaveChangesAsync(context.CancellationToken);
        }
        catch (Exception e)
        {
            var httpContext = context.GetHttpContext();
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }

        return await Task.FromResult(new UpdateRoomReply {Message = "room was updated"});
    }

    public override async Task<DeleteRoomReply> DeleteRoom(DeleteRoomRequest request, ServerCallContext context)
    {
        try
        {
            var room = await _context.Rooms
                .SingleAsync(r => r.RoomId.ToString() == request.RoomId,
                    context.CancellationToken);
            
            _context.Rooms.Remove(room);
            
            await _context.SaveChangesAsync(context.CancellationToken);
        }
        catch (Exception e)
        {
            var httpContext = context.GetHttpContext();
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }

        return await Task.FromResult(new DeleteRoomReply {Message = "room was delete"});
    }
}