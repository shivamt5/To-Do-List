using Quartz;
using SendGrid.Helpers.Mail;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList
{
    public class DemoJob : IJob
    {
        private readonly ApplicationDbContext _context;
        private readonly List<Tasks> tasksList = new List<Tasks>();
        private readonly List<ApplicationUser> userList = new List<ApplicationUser>();

        public DemoJob(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task Execute(IJobExecutionContext context)
        {
            List<Tasks> tasksList = _context.tasks.ToList();
            List<ApplicationUser> userList = _context.ApplicationUser.ToList();

            var msg = new SendGridMessage();
            foreach (var task in tasksList)
            {
                // Calculate days remaining for task due date
                int daysRemaining = (task.DueDate - DateTime.Now).Days;
                //int hoursRemaining = (task.DueDate - DateTime.Now).Hours;

                // If due date is within 2 days, send email notification
                if (daysRemaining <= 2)
                {
                    string taskName = task.Title;
                    string userID = task.UserId;

                    ApplicationUser user = task.User;
                    //var tasks = _context.tasks.Where(t =>t.UserId == currentUser.Id);
                    //string to = userList.Where(t=> task.UserId == userList.);

                    string to = user.Email;
                    string name = user.UserName;
                    
                    msg = new SendGridMessage
                    {
                        From = new EmailAddress("shivamtivrekar005@gmail.com", "Admin"),
                        Subject = $"Approaching Task Deadline: {taskName}",
                        PlainTextContent = $"This is a reminder that the task '{taskName}' is due in {daysRemaining} days."
                    };
                    msg.AddTo(new EmailAddress(to, name));
                }
            }
            //Write your custom code here
            return Task.FromResult(true);
        }
    }
}
