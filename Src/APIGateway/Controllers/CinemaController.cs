using Cinema;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Controllers;

[ApiController]
[Route("api")]
public class CinemaController : ControllerBase
{
    private readonly string _address = GrpcAddresses.Cinema;

    public CinemaController()
    {
    }

    [HttpGet("cinemas/{id}")]
    public async Task<ActionResult> GetByIdCinema(string id)
    {
        using var channel = GrpcChannel.ForAddress(_address);
        var client = new CinemaService.CinemaServiceClient(channel);

        var reply = await client.GetByIdCinemaAsync(new GetByIdCinemaRequest {CinemaId = id});
        return Ok(reply);
    }

    [HttpGet("cinemas")]
    public async Task<ActionResult> GetAllCinema()
    {
        using var channel = GrpcChannel.ForAddress(_address);
        var client = new CinemaService.CinemaServiceClient(channel);

        var reply = await client.GetAllCinemaAsync(new GetAllCinemaRequest());
        return Ok(reply);
    }

    [HttpPost("cinemas")]
    public async Task<ActionResult> CreateCinema(CreateCinemaRequest request)
    {
        using var channel = GrpcChannel.ForAddress(_address);
        var client = new CinemaService.CinemaServiceClient(channel);

        var reply = await client.CreateCinemaAsync(request);
        return Ok(reply);
    }

    [HttpPut("cinemas")]
    public async Task<ActionResult> UpdateCinema(UpdateCinemaRequest request)
    {
        using var channel = GrpcChannel.ForAddress(_address);
        var client = new CinemaService.CinemaServiceClient(channel);

        var reply = await client.UpdateCinemaAsync(request);
        return Ok(reply);
    }

    [HttpDelete("cinemas/{cinemaId}")]
    public async Task<ActionResult> DeleteCinema(string cinemaId)
    {
        using var channel = GrpcChannel.ForAddress(_address);
        var client = new CinemaService.CinemaServiceClient(channel);

        var reply = await client.DeleteCinemaAsync(new DeleteCinemaRequest {CinemaId = cinemaId});
        return Ok(reply);
    }
}