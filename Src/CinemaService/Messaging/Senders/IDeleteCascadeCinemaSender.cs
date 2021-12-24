namespace CinemaService.Messaging.Senders;

public interface IDeleteCascadeCinemaSender
{
    void Send(string id);
}