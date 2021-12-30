namespace Domain.Interfaces;

public interface ISender<T>
{
    Task Send(T msg, CancellationToken token);
}