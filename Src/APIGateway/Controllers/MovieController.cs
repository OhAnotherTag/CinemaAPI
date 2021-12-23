using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Movie;

namespace APIGateway.Controllers;


[ApiController]
[Route("api")]
public class MovieController : ControllerBase
{
    private readonly string _address = GrpcAddresses.Movie;

    public MovieController()
    {
    }
    
    [HttpGet("movies/{id}")]
    public async Task<ActionResult> GetByIdMovie(string id)
    {
        using var channel = GrpcChannel.ForAddress(_address);
        var client = new MovieService.MovieServiceClient(channel);

        var reply = await client.GetByIdMovieAsync(new GetByIdMovieRequest {MovieId = id});
        return Ok(reply);
    }
    
    [HttpGet("movies")]
    public async Task<ActionResult> GetAllMovie()
    {
        using var channel = GrpcChannel.ForAddress(_address);
        var client = new MovieService.MovieServiceClient(channel);

        var reply = await client.GetAllMovieAsync(new GetAllMovieRequest());
        return Ok(reply);
    }
    
    [HttpPost("movies")]
    public async Task<ActionResult> CreateMovie(CreateMovieRequest request)
    {
        using var channel = GrpcChannel.ForAddress(_address);
        var client = new MovieService.MovieServiceClient(channel);

        var reply = await client.CreateMovieAsync(request);
        return Ok(reply);
    }
    
    [HttpPost("movies")]
    public async Task<ActionResult> UpdateCinema(UpdateMovieRequest request)
    {
        using var channel = GrpcChannel.ForAddress(_address);
        var client = new MovieService.MovieServiceClient(channel);

        var reply = await client.UpdateMovieAsync(request);
        return Ok(reply);
    }

    [HttpPost("movies/{movieId}")]
    public async Task<ActionResult> DeleteMovie(string movieId)
    {
        using var channel = GrpcChannel.ForAddress(_address);
        var client = new MovieService.MovieServiceClient(channel);

        var reply = await client.DeleteMovieAsync(new DeleteMovieRequest {MovieId = movieId});
        return Ok(reply);
    }
}