using Domain;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Movie;
using MovieService.Model;

namespace MovieService.Api;

public class MovieController : Movie.MovieService.MovieServiceBase
{
    private readonly MovieContext _context;

    public MovieController(MovieContext context)
    {
        _context = context;
    }

    public override async Task<GetByIdMovieReply> GetByIdMovie(GetByIdMovieRequest request, ServerCallContext context)
    {
        GetByIdMovieReply res;
        try
        {
            var movie = await _context.Movies.SingleAsync(
                m => m.MovieId.ToString() == request.MovieId,
                context.CancellationToken
            );

            res = await Task.FromResult(new GetByIdMovieReply
            {
                Format = movie.Format.ToString().ToLower(),
                MovieId = movie.MovieId.ToString(),
                Plot = movie.Plot,
                ReleaseDay = movie.ReleaseDate.Day,
                ReleaseMonth = movie.ReleaseDate.Month,
                ReleaseYear = movie.ReleaseDate.Year,
                Runtime = movie.Runtime,
                Title = movie.Title,
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

    public override async Task<GetAllMovieReply> GetAllMovie(GetAllMovieRequest request, ServerCallContext context)
    {
        GetAllMovieReply res;
        try
        {
            var movies = await _context.Movies.ToListAsync(context.CancellationToken);
            res = await Task.FromResult(new GetAllMovieReply());

            foreach (var movie in movies)
            {
                res.Movies.Add(new Movie.Movie
                {
                    Format = movie.Format.ToString().ToLower(),
                    MovieId = movie.MovieId.ToString(),
                    Plot = movie.Plot,
                    ReleaseDay = movie.ReleaseDate.Day,
                    ReleaseMonth = movie.ReleaseDate.Month,
                    ReleaseYear = movie.ReleaseDate.Year,
                    Runtime = movie.Runtime,
                    Title = movie.Title,
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

    public override async Task<CreateMovieReply> CreateMovie(CreateMovieRequest request, ServerCallContext context)
    {
        try
        {
            var movie = new MovieModel
            {
                Title = request.Title,
                Plot = request.Plot,
                ReleaseDate = DateOnly.Parse($"{request.ReleaseYear}-{request.ReleaseMonth}-{request.ReleaseDay}"),
                Runtime = request.Runtime,
                Format = request.Format switch
                {
                    "classic" => Format.Classic,
                    "imax" => Format.Imax,
                    _ => throw new ArgumentException("movie format not supported")
                }
            };

            movie.Validate();

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync(context.CancellationToken);
        }
        catch (Exception e)
        {
            var httpContext = context.GetHttpContext();
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }

        return await Task.FromResult(new CreateMovieReply {Message = "new movie was created"});
    }

    public override async Task<UpdateMovieReply> UpdateMovie(UpdateMovieRequest request, ServerCallContext context)
    {
        try
        {
            var movie = await _context.Movies
                .SingleAsync(m => m.MovieId.ToString() == request.MovieId,
                    context.CancellationToken);

            var year = request.ReleaseYear >= 2021 ? movie.ReleaseDate.Year : request.ReleaseYear;
            var month = request.ReleaseMonth >= 0 ? movie.ReleaseDate.Month : request.ReleaseMonth;
            var day = request.ReleaseDay >= 0 ? movie.ReleaseDate.Day : request.ReleaseDay;

            movie.Title = request.Title ?? movie.Title;
            
            movie.Plot = request.Plot ?? movie.Plot;
            
            movie.ReleaseDate = DateOnly.Parse($"{year}-{month}-{day}");
            
            movie.Runtime = request.Runtime >= 0 ? movie.Runtime : request.Runtime;
            
            movie.Format = request.Format is null
                ? movie.Format
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

        return await Task.FromResult(new UpdateMovieReply {Message = "movie was updated"});
    }

    public override async Task<DeleteMovieReply> DeleteMovie(DeleteMovieRequest request, ServerCallContext context)
    {
        try
        {
            var movie = await _context.Movies
                .SingleAsync(m => m.MovieId.ToString() == request.MovieId,
                    context.CancellationToken);

            _context.Movies.Remove(movie);

            await _context.SaveChangesAsync(context.CancellationToken);
        }
        catch (Exception e)
        {
            var httpContext = context.GetHttpContext();
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }

        return await Task.FromResult(new DeleteMovieReply {Message = "cinema was deleted"});
    }
}