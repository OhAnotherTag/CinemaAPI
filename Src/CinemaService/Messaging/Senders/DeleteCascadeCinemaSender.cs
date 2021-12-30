using System.Text;
using Domain.Contracts;
using Domain.Interfaces;
using MassTransit;
using RabbitMQ.Client;

namespace CinemaService.Messaging.Senders;

public class DeleteCascadeCinemaSender : ISender<CinemaDeleted>
{
    private readonly IPublishEndpoint _publishEndpoint;
    public DeleteCascadeCinemaSender(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task Send(CinemaDeleted message, CancellationToken token)
    {
        await _publishEndpoint.Publish(message,token);
    }
}