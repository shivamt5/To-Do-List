using Humanizer;
using Mailjet.Client.Resources;
using MailKit.Security;
using MimeKit;
using ToDoList.Models;

namespace ToDoList.Services
{
    public class EmailService : IEmailService
    {
        public async Task<bool> SendForgotPasswordEmail(string name, string to, string subject, string body)
        {
            try
            {
                MimeMessage emailMessage = new MimeMessage();
                MailboxAddress emailFrom = new MailboxAddress("Support@To-Do-List", "shivamtivrekar@outlook.com");
                emailMessage.From.Add(emailFrom);

                MailboxAddress emailTo = new MailboxAddress(name, to);
                emailMessage.To.Add(emailTo);

                emailMessage.Subject = subject;
                BodyBuilder emailBodyBuilder = new BodyBuilder();
                emailBodyBuilder.TextBody = body;
                emailMessage.Body = emailBodyBuilder.ToMessageBody();

                MailKit.Net.Smtp.SmtpClient emailClient = new MailKit.Net.Smtp.SmtpClient();
                emailClient.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);
                emailClient.Authenticate("shivamtivrekar@outlook.com", "shivam@005");
                emailClient.Send(emailMessage);
                emailClient.Disconnect(true);
                emailClient.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception("Failed to send email", ex);
            }

        }

        public async Task<bool> SendReminderEmail(string taskName, ApplicationUser user, int daysRemaining, MimeMessage emailMessage, MailKit.Net.Smtp.SmtpClient emailClient)
        {
            try
            {
                string to = user.Email;
                string name = user.UserName;

                MailboxAddress emailTo = new MailboxAddress(name, to);
                emailMessage.To.Add(emailTo);

                emailMessage.Subject = $"Approaching Task Deadline: {taskName}";

                BodyBuilder emailBodyBuilder = new BodyBuilder();
                emailBodyBuilder.TextBody = $"This is a reminder that the task '{taskName}' is due in {daysRemaining} days.";
                emailMessage.Body = emailBodyBuilder.ToMessageBody();

                emailClient.Send(emailMessage);

                return true;
            }
            catch (Exception ex) 
            {
                return false;
                throw new Exception("Failed to send email", ex);
            }
        }
    }
}
