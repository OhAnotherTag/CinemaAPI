using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Room;

namespace APIGateway.Controllers;

[ApiController]
[Route("api")]
public class RoomController : ControllerBase
{
    private readonly string _address = GrpcAddresses.Room;

    public RoomController()
    {
        
    }

        [HttpGet("cinemas/{cinemaId}/rooms/{roomId}")]
    public async Task<ActionResult> GetByIdRoom(string cinemaId, string roomId)
    {
        using var channel = GrpcChannel.ForAddress(_address);
        var client = new RoomService.RoomServiceClient(channel);

        var reply = await client.GetByIdRoomAsync(new GetByIdRoomRequest {RoomId = roomId});
        return Ok(reply);
    }
    
    [HttpGet("cinemas/{cinemaId}/rooms/")]
    public async Task<ActionResult> GetAllRoom(string cinemaId)
    {
        using var channel = GrpcChannel.ForAddress(_address);
        var client = new RoomService.RoomServiceClient(channel);

        var reply = await client.GetAllRoomAsync(new GetAllRoomRequest());
        return Ok(reply);
    }
    
    [HttpPost("cinemas/{cinemaId}/rooms/")]
    public async Task<ActionResult> CreateRoom(CreateRoomRequest request)
    {
        using var channel = GrpcChannel.ForAddress(_address);
        var client = new RoomService.RoomServiceClient(channel);

        var reply = await client.CreateRoomAsync(request);
        return Ok(reply);
    }
    
    [HttpPut("cinemas/{cinemaId}/rooms")]
    public async Task<ActionResult> UpdateCinema(UpdateRoomRequest request)
    {
        using var channel = GrpcChannel.ForAddress(_address);
        var client = new RoomService.RoomServiceClient(channel);

        var reply = await client.UpdateRoomAsync(request);
        return Ok(reply);
    }

    [HttpDelete("cinemas/{cinemaId}rooms/{roomId}")]
    public async Task<ActionResult> DeleteRoom(string roomId)
    {
        using var channel = GrpcChannel.ForAddress(_address);
        var client = new RoomService.RoomServiceClient(channel);

        var reply = await client.DeleteRoomAsync(new DeleteRoomRequest {RoomId = roomId});
        return Ok(reply);
    }
}