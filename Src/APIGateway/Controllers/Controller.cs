namespace APIGateway.Controllers;

public static class GrpcAddresses
{
    private static readonly ArgumentException InvalidAddress =
        new ("grpc connection must need a valid address");

    public static readonly string Cinema = Environment.GetEnvironmentVariable("CINEMA_SERVICE_ADDR")
                                           ?? throw InvalidAddress;

    public static readonly string Room = Environment.GetEnvironmentVariable("ROOM_SERVICE_ADDR")
                                         ?? throw InvalidAddress;

    public static readonly string Session = Environment.GetEnvironmentVariable("SESSION_SERVICE_ADDR")
                                            ?? throw InvalidAddress;

    public static readonly string Movie = Environment.GetEnvironmentVariable("MOVIE_SERVICE_ADDR")
                                          ?? throw InvalidAddress;
}