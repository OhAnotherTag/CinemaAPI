using Microsoft.EntityFrameworkCore;

namespace SessionService.Messaging.Receiver;

using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Model;

public class DeleteCascadeRoomReceiver : BackgroundService
{
    private static readonly ArgumentException InvalidEnvVariables =
        new("provide the necessary env variables");

    private readonly string _hostname =
        Environment.GetEnvironmentVariable("MQ_HOST") ?? throw InvalidEnvVariables;

    private readonly string _username =
        Environment.GetEnvironmentVariable("MQ_USERNAME") ?? throw InvalidEnvVariables;
    
    private readonly string _password =
        Environment.GetEnvironmentVariable("MQ_PASSWORD") ?? throw InvalidEnvVariables;

    private const string QueueName = "room-delete";

    private readonly SessionContext _context;
    private IConnection? _connection;
    private IModel? _channel;

    public DeleteCascadeRoomReceiver(SessionContext context)
    {
        _context = context;
        
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
            
            Console.WriteLine("CONTENT: " + content);
            
            var room =  await _context.Sessions
                .SingleOrDefaultAsync(r => r.RoomId.ToString() == content, cancellationToken: stoppingToken);

            if (room != null)
            {
                Console.WriteLine("ROOM ID: " + room.RoomId);
                
                _context.Sessions.Remove(room);
                
                await _context.SaveChangesAsync(stoppingToken);
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