using Grpc.Core;
using Cinema;
using CinemaService.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using CinemaService.Messaging.Senders;
using Domain.Contracts;
using Domain.Interfaces;

namespace CinemaService.Api;

public class CinemaController : Cinema.CinemaService.CinemaServiceBase
{
    private readonly CinemaContext _context;
    private readonly ISender<CinemaDeleted> _sender;

    public CinemaController(CinemaContext context, ISender<CinemaDeleted> sender)
    {
        _context = context;
        _sender = sender;
    }

    public override async Task<GetByIdCinemaReply> GetByIdCinema(GetByIdCinemaRequest request,
        ServerCallContext context)
    {
        GetByIdCinemaReply res;
        try
        {
            var cinema = await _context.Cinemas.SingleAsync(
                c => c.CinemaId.ToString() == request.CinemaId,
                context.CancellationToken
            );

            res = await Task.FromResult(new GetByIdCinemaReply
            {
                CinemaId = cinema.CinemaId.ToString(),
                Name = cinema.Name,
                Location = cinema.Location
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

    public override async Task<GetAllCinemaReply> GetAllCinema(GetAllCinemaRequest request, ServerCallContext context)
    {
        GetAllCinemaReply res;
        try
        {
            var cinema = await _context.Cinemas.ToListAsync(context.CancellationToken);
            res = await Task.FromResult(new GetAllCinemaReply());

            foreach (var c in cinema)
            {
                res.Cinemas.Add(new Cinema.Cinema
                {
                    CinemaId = c.CinemaId.ToString(),
                    Name = c.Name,
                    Location = c.Location
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

    public override async Task<CreateCinemaReply> CreateCinema(CreateCinemaRequest request, ServerCallContext context)
    {
        try
        {
            var cinema = new CinemaModel
            {
                Location = request.Location,
                Name = request.Name,
            };

            _context.Cinemas.Add(cinema);

            await _context.SaveChangesAsync(context.CancellationToken);
        }
        catch (Exception e)
        {
            var httpContext = context.GetHttpContext();
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }

        return await Task.FromResult(new CreateCinemaReply {Message = "new cinema was created"});
    }

    public override async Task<UpdateCinemaReply> UpdateCinema(UpdateCinemaRequest request, ServerCallContext context)
    {
        try
        {
            var cinema = await _context.Cinemas
                .SingleAsync(c => c.CinemaId.ToString() == request.CinemaId,
                    context.CancellationToken);

            cinema.Location = request.Location ?? cinema.Location;
            cinema.Name = request.Name ?? cinema.Name;

            await _context.SaveChangesAsync(context.CancellationToken);
        }
        catch (Exception e)
        {
            var httpContext = context.GetHttpContext();
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }

        return await Task.FromResult(new UpdateCinemaReply {Message = "cinema was delete"});
    }

    public override async Task<DeleteCinemaReply> DeleteCinema(DeleteCinemaRequest request, ServerCallContext context)
    {
        try
        {
            var cinema = await _context.Cinemas
                .SingleAsync(c => c.CinemaId.ToString() == request.CinemaId,
                    context.CancellationToken);
            
            _context.Cinemas.Remove(cinema);
            
            await _context.SaveChangesAsync(context.CancellationToken);

            await _sender.Send(new CinemaDeleted
            {
                CinemaId = request.CinemaId
            }, context.CancellationToken);
        }
        catch (Exception e)
        {
            var httpContext = context.GetHttpContext();
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }

        return await Task.FromResult(new DeleteCinemaReply {Message = "cinema was delete"});
    }
}