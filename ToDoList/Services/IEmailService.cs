using MimeKit;
using ToDoList.Models;

namespace ToDoList.Services
{
    public interface IEmailService
    {
       Task<bool> SendForgotPasswordEmail(string name, string to, string subject, string body);
       Task<bool> SendReminderEmail(string taskName, ApplicationUser user, int daysRemaining, MimeMessage emailMessage, MailKit.Net.Smtp.SmtpClient emailClient);
    }
}
