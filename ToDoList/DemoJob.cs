using MailKit.Security;
using MimeKit;
using Quartz;
using System.Threading.Tasks;
using ToDoList.Data;
using ToDoList.Models;
using MailKit.Net.Smtp;
using Humanizer;
using ToDoList.Services;

namespace ToDoList
{
    public class DemoJob : IJob
    {
        private readonly ApplicationDbContext _context;
        private readonly List<Tasks> tasksList = new List<Tasks>();
        private readonly List<ApplicationUser> userList = new List<ApplicationUser>();
        private readonly IEmailService _emailService;

        public DemoJob(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            List<Tasks> tasksList = _context.tasks.ToList();
            List<ApplicationUser> userList = _context.ApplicationUser.ToList();
            MimeMessage emailMessage = new MimeMessage();

            MailboxAddress emailFrom = new MailboxAddress("Support@To-Do-List", "shivamtivrekar@outlook.com");
            emailMessage.From.Add(emailFrom);

            MailKit.Net.Smtp.SmtpClient emailClient = new SmtpClient();
            emailClient.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);
            emailClient.Authenticate("shivamtivrekar@outlook.com", "shivam@005");

            foreach (var task in tasksList)
            {
                int daysRemaining = (task.DueDate - DateTime.Now).Days;

                // If due date is within 2 days, send email notification
                if (daysRemaining <= 2)
                {
                    string taskName = task.Title;
                    //string userID = task.UserId;
                    ApplicationUser user = task.User;

                    var EmailSentSuccessfully = await _emailService.SendReminderEmail(taskName, user, daysRemaining, emailMessage, emailClient);
                }
            }
            emailClient.Disconnect(true);
            emailClient.Dispose();
        }
    }
}