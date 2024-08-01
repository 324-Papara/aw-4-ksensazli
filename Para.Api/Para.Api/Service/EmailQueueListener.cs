using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Para.Api.Service;

public class EmailQueueListener : BackgroundService
{
    private readonly IRabbitMQClient _rabbitMqClient;
    private readonly IEmailSender _emailSender;
    private readonly RabbitMQConfig _config;

    public EmailQueueListener(IRabbitMQClient rabbitMqClient, IEmailSender emailSender, IOptions<RabbitMQConfig> config)
    {
        _rabbitMqClient = rabbitMqClient;
        _emailSender = emailSender;
        _config = config.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var connection = new ConnectionFactory() { HostName = _config.HostName, UserName = _config.UserName, Password = _config.Password }.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: _config.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var emailMessage = JsonConvert.DeserializeObject<EmailMessage>(message);
                await _emailSender.SendEmailAsync(emailMessage);
            };

            channel.BasicConsume(queue: _config.QueueName, autoAck: true, consumer: consumer);

            await Task.Delay(5000, stoppingToken);
        }
    }
}
