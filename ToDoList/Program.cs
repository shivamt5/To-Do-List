using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using Hangfire.SQLite;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Extensions.DependencyInjection;
using SendGrid.Helpers.Mail;
using ToDoList.Data;
using ToDoList.Models;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authentication.OAuth;
using ToDoList;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSendGrid(options => options.ApiKey = builder.Configuration["SendGridApiKey"]);

builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = "309873603729-n3g25uekn0ck4go9hm0vptkh11sa2h8p.apps.googleusercontent.com";
    options.ClientSecret = "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";
    
    options.CallbackPath = "/signin-google";
    // Add the prompt parameter to request reauthentication
    options.AuthorizationEndpoint += "?prompt=select_account";
});

builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "902774317497639";
    options.AppSecret = "0799a633ed8e7ff4ca9113a388716e3b";
    options.CallbackPath = "/signin-facebook?auth_type=reauthenticate"; // Add auth_type as a query string parameter
    
    //options.Events = new OAuthEvents
    //{
    //    OnRedirectToAuthorizationEndpoint = context =>
    //    {
    //        // Add the auth_type parameter to force reauthentication
    //        context.RedirectUri += "&auth_type=reauthenticate";
    //        return Task.CompletedTask;
    //    }
    //};
});

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionScopedJobFactory();
    var jobKey = new JobKey("DemoJob");
    q.AddJob<DemoJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("DemoJob-trigger")
        .WithCronSchedule("0/1 * * * * ?"));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Add Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

//builder.Services.AddHangfire(configuration => configuration
//    .UseSimpleAssemblyNameTypeSerializer()
//    .UseRecommendedSerializerSettings()
//    .UseInMemoryStorage());

//builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use Identity
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// host dashboard at "/"
//app.MapHangfireDashboard("");

//await app.StartAsync();

//RecurringJob.AddOrUpdate<EmailJob>(emailJob => emailJob.SendEmail(), "*/15 * * * * *");

//await app.WaitForShutdownAsync();

//public class EmailJob
//{
//    private readonly ILogger<EmailJob> logger;
//    private readonly ISendGridClient sendGridClient;

//    public EmailJob(ILogger<EmailJob> logger, ISendGridClient sendGridClient)
//    {
//        this.logger = logger;
//        this.sendGridClient = sendGridClient;
//    }

//    public async Task SendEmail()
//    {
//        List<Tasks> tasks = new List<Tasks>();
//        var msg = new SendGridMessage();
//        foreach (var task in tasks)
//        {
//            // Calculate days remaining for task due date
//            int daysRemaining = (task.DueDate - DateTime.Now).Days;
//            int hoursRemaining = (task.DueDate - DateTime.Now).Hours;

//            // If due date is within 2 days, send email notification
//            if (daysRemaining <= 2)
//            {
//                string to = task.User.Email;
//                string taskName = task.Title;
//                DateTime dueDate = task.DueDate;
//                string subject = $"Approaching Task Deadline: {taskName}";
//                string body = $"This is a reminder that the task '{taskName}' is due in {daysRemaining} days.";

//                msg = new SendGridMessage
//                {
//                    From = new EmailAddress("shivamtivrekar005@gmail.com", "Admin"),
//                    Subject = $"Approaching Task Deadline: {taskName}",
//                    PlainTextContent = $"This is a reminder that the task '{taskName}' is due in {hoursRemaining} days."
//                };
//                msg.AddTo(new EmailAddress(task.User.Email, task.User.UserName));
//            }
//        }
//    }
//}