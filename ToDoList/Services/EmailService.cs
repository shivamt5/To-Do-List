//using System.Net;
//using System.Net.Mail;
//using Microsoft.Extensions.Configuration;

//// Inject IConfiguration in your service or controller
//public class EmailService
//{
//    private readonly IConfiguration _configuration;

//    public EmailService(IConfiguration configuration)
//    {
//        _configuration = configuration;
//    }

//    public void SendEmail(string to, string subject, string body)
//    {
//        // Get SMTP settings from appsettings.json
//        var smtpSettings = _configuration.GetSection("SmtpSettings")
//            .Get<SmtpSettings>();

//        // Create SmtpClient
//        using (var client = new SmtpClient(smtpSettings.Server, smtpSettings.Port))
//        {
//            client.EnableSsl = smtpSettings.UseSsl;
//            client.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);

//            // Create MailMessage
//            using (var message = new MailMessage())
//            {
//                message.From = new MailAddress(smtpSettings.Username);
//                message.To.Add(to);
//                message.Subject = subject;
//                message.Body = body;

//                // Send email
//                client.Send(message);
//            }
//        }
//    }
//}

//// SmtpSettings model to hold SMTP settings
//public class SmtpSettings
//{
//    public string Server { get; set; }
//    public int Port { get; set; }
//    public string Username { get; set; }
//    public string Password { get; set; }
//    public bool UseSsl { get; set; }
//}

