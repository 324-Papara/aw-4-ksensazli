namespace Para.Api.Service;

public interface IEmailSender
{
    Task SendEmailAsync(EmailMessage message);
}