using System.Text;
using RabbitMQ.Client;

namespace CinemaService.Messaging.Senders;

public class DeleteCascadeCinemaSender : IDeleteCascadeCinemaSender
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


    private IConnection? _connection;

    public DeleteCascadeCinemaSender()
    {
        CreateConnection();
    }

    public void Send(string id)
    {
        if (!ConnectionExists()) return;

        using var channel = _connection?.CreateModel();

        channel?.QueueDeclare(QueueName, false, false, false, null);
        var body = Encoding.UTF8.GetBytes(id);
        
        channel.BasicPublish("", QueueName, null, body);
        
        Console.WriteLine("message sent");
    }

    private void CreateConnection()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostname,
                UserName = _username,
                Password = _password,
            };

            _connection = factory.CreateConnection();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Could not establish connection: {e.Message}");
        }
    }

    private bool ConnectionExists()
    {
        if (_connection is not null)
        {
            return true;
        }

        CreateConnection();

        return _connection is not null;
    }
}