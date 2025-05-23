using Microsoft.Extensions.Options;
using ServiceContracts.Options;
using ServiceContracts.ServicesContracts;
using System.Net;
using System.Net.Mail;

namespace Services;
public class EmailService : IEmailServices
{
    private readonly EmailOptions _emailConfig;
    public EmailService(IOptions<EmailOptions> emailConfig)
    {
        _emailConfig = emailConfig.Value;
    }
    public async Task SendEmail(string to , string subject , string body)
    {
        var email = _emailConfig.From;
        var password = _emailConfig.Password;
        var host  = _emailConfig.SmtpServer;
        var port = _emailConfig.Port;
        var smtpClient = new SmtpClient(host, port)
        {
            UseDefaultCredentials = false,
            EnableSsl = true,
            Credentials = new NetworkCredential(email, password)
        };

        var message = new MailMessage(email, to, subject, body)
        {
            IsBodyHtml = true
        };
        await smtpClient.SendMailAsync(message);
    }

}