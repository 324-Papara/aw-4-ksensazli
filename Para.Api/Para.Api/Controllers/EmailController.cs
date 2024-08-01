using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Para.Api.Service;

namespace Para.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class EmailController : ControllerBase
{
    private readonly IRabbitMQClient _rabbitMqClient;

    public EmailController(IRabbitMQClient rabbitMqClient)
    {
        _rabbitMqClient = rabbitMqClient;
    }

    [HttpPost]
    [Route("send")]
    public IActionResult SendEmail([FromBody] EmailMessage emailMessage)
    {
        var message = JsonConvert.SerializeObject(emailMessage);
        _rabbitMqClient.Publish(message);
        return Ok("Message published to RabbitMQ");
    }

    [HttpPost]
    [Route("schedule")]
    public IActionResult ScheduleEmail([FromBody] EmailMessage emailMessage, int delayInSeconds)
    {
        BackgroundJob.Schedule(() => _rabbitMqClient.Publish(JsonConvert.SerializeObject(emailMessage)), TimeSpan.FromSeconds(delayInSeconds));
        return Ok("Email scheduled with Hangfire");
    }
}
