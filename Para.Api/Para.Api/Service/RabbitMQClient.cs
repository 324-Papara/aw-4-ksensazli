using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Para.Api.Service;

public class RabbitMQClient : IRabbitMQClient
{
    private readonly RabbitMQConfig _config;

    public RabbitMQClient(IOptions<RabbitMQConfig> config)
    {
        _config = config.Value;
    }

    public void Publish(string message)
    {
        var factory = new ConnectionFactory() { HostName = _config.HostName, UserName = _config.UserName, Password = _config.Password };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: _config.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: "", routingKey: _config.QueueName, basicProperties: null, body: body);
    }
}