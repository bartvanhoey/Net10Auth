using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Net10Auth.API.Services.Email;

public class ProductionEmailSender(string googleEmail, string googleAppPassword) : IEmailSender
{
    private readonly string _googleEmail = googleEmail;
    private readonly string _googleAppPassword = googleAppPassword;

    public async Task SendEmailAsync(string emailTo, string emailSubject, string emailBody)
    {
        MailMessage mailMessage = new()
        {
            From = new MailAddress(_googleEmail)
        };
        mailMessage.To.Add(new MailAddress(emailTo));
        mailMessage.Subject = emailSubject;
        mailMessage.IsBodyHtml = true;
        mailMessage.Body = emailBody;

        SmtpClient client = new()
        {
            EnableSsl = true,
            Credentials = new System.Net.NetworkCredential(_googleEmail, _googleAppPassword),
            UseDefaultCredentials = false,
            Host = "smtp.gmail.com",
            Port = 587
        };
        await Task.CompletedTask;

        try
        {
            client.Send(mailMessage);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }

    }
}