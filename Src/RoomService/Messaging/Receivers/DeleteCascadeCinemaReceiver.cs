using System.Text;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RoomService.Messaging.Sender;
using RoomService.Model;

namespace RoomService.Messaging.Receivers;

public class DeleteCascadeCinemaReceiver : BackgroundService
{
    private static readonly ArgumentException InvalidEnvVariables =
        new("provide the necessary env variables");

    private readonly string _hostname =
        Environment.GetEnvironmentVariable("MQ_HOST") ?? throw InvalidEnvVariables;

    private readonly string _username =
        Environment.GetEnvironmentVariable("MQ_USERNAME") ?? throw InvalidEnvVariables;
    
    private readonly string _password =
        Environment.GetEnvironmentVariable("MQ_PASSWORD") ?? throw InvalidEnvVariables;

    private const string QueueName = "cinema-delete";

    private readonly RoomContext _context;
    private IConnection? _connection;
    private IModel? _channel;
    private readonly IDeleteCascadeRoomSender _sender;

    public DeleteCascadeCinemaReceiver(IDeleteCascadeRoomSender sender, RoomContext context)
    {
        _context = context;
        _sender = sender;
        
        InitializeRabbitMqListener();
    }

    private void InitializeRabbitMqListener()
    {
        var factory = new ConnectionFactory
        {
            HostName = _hostname,
            UserName = _username,
            Password = _password,
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel?.QueueDeclare(QueueName, false, false, false, null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (ch, ea) =>
        {
            Console.WriteLine("message received");
            
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            
            Console.WriteLine("MESSAGE CONTENT: " + content);
            
            var room =  await _context.Rooms
                .SingleOrDefaultAsync(r => r.CinemaId.ToString() == content, cancellationToken: stoppingToken);

            if (room is not null)
            {
                Console.WriteLine("CINEMA ID: " + room.CinemaId);
                
                _context.Rooms.Remove(room);

                await _context.SaveChangesAsync(stoppingToken);
                
                _sender.Send(room.RoomId.ToString());
            }

            _channel?.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(QueueName, false, consumer);
        
        return Task.CompletedTask;
    }
    
    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}