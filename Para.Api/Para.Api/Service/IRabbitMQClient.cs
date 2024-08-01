namespace Para.Api.Service;

public interface IRabbitMQClient
{
    void Publish(string message);
}