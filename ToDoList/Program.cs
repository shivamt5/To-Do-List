using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ToDoList.Data;
using ToDoList.Models;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authentication.OAuth;
using ToDoList;
using Quartz;
using ToDoList.Services;

var builder = WebApplication.CreateBuilder(args);

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
});

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionScopedJobFactory();
    var jobKey = new JobKey("DemoJob");
    q.AddJob<DemoJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("DemoJob-trigger")
        .WithCronSchedule("0 00 11 ? * *"));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddTransient<IEmailService, EmailService>();

// Add Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

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