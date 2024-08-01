using MimeKit;

namespace Para.Api.Service;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(EmailMessage message)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("Your Name", _configuration["Smtp:Username"]));
        emailMessage.To.Add(new MailboxAddress("", message.To));
        emailMessage.Subject = message.Subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = message.Body };
        emailMessage.Body = bodyBuilder.ToMessageBody();

        using var client = new MailKit.Net.Smtp.SmtpClient();
        await client.ConnectAsync(_configuration["Smtp:Server"], int.Parse(_configuration["Smtp:Port"]), true);
        await client.AuthenticateAsync(_configuration["Smtp:Username"], _configuration["Smtp:Password"]);
        await client.SendAsync(emailMessage);
        await client.DisconnectAsync(true);
    }
}