using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList
{
    public class MyBackgroundService : BackgroundService
    {
        private readonly ILogger<MyBackgroundService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly List<Tasks> tasksList = new List<Tasks>();

        public MyBackgroundService(ILogger<MyBackgroundService> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("From MyBackgroundService: ExecuteAsync");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

                List<Tasks> tasksList = _context.tasks.ToList();
                //List<Tasks> tasks = new List<Tasks>();
                var msg = new SendGridMessage();
                foreach (var task in tasksList)
                {
                    Console.WriteLine("task: ", task.Title);
                    // Calculate days remaining for task due date
                    int daysRemaining = (task.DueDate - DateTime.Now).Days;
                    //int hoursRemaining = (task.DueDate - DateTime.Now).Hours;

                    // If due date is within 2 days, send email notification
                    if (daysRemaining <= 2)
                    {
                        //string to = task.User.Email;
                        string taskName = task.Title;
                        //DateTime dueDate = task.DueDate;
                        //string subject = $"Approaching Task Deadline: {taskName}";
                        //string body = $"This is a reminder that the task '{taskName}' is due in {daysRemaining} days.";

                        msg = new SendGridMessage
                        {
                            From = new EmailAddress("shivamtivrekar005@gmail.com", "Admin"),
                            Subject = $"Approaching Task Deadline: {taskName}",
                            PlainTextContent = $"This is a reminder that the task '{taskName}' is due in {daysRemaining} days."
                        };
                        msg.AddTo(new EmailAddress(task.User.Email, task.User.UserName));
                    }
                }
            }
            //throw new NotImplementedException();
        }
    }
}