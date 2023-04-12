using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public TasksController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize]
        public async Task<IActionResult> Index(string sortOrder)
        {
            var currentUser = await _userManager.GetUserAsync(User);
           
            if (currentUser == null)
            {
                return NotFound();
            }

            var tasks = _context.tasks.Where(t =>t.UserId == currentUser.Id);

            switch (sortOrder)
            {
                case "Title":
                    tasks = tasks.OrderBy(t => t.Title);
                    break;
                case "DueDate":
                    tasks = tasks.OrderBy(t => t.DueDate);
                    break;
                case "Priority":
                    tasks = tasks.OrderBy(t => t.Priority);
                    break;
                default:
                    tasks = tasks.OrderBy(t => t.Title);
                    break;
            }

            var sortedTasks = await tasks.ToListAsync();

            return View(sortedTasks);
        }

        [Authorize]
        public async Task<IActionResult> CompletedTasks(string sortOrder)
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);

            // Retrieve completed tasks for the user from the database
            var completedTasks = _context.tasks.Where(t => t.IsCompleted && t.UserId == user.Id);

            switch (sortOrder)
            {
                case "Title":
                    completedTasks = completedTasks.OrderBy(t => t.Title);
                    break;
                case "DueDate":
                    completedTasks = completedTasks.OrderBy(t => t.DueDate);
                    break;
                case "Priority":
                    completedTasks = completedTasks.OrderBy(t => t.Priority);
                    break;
                default:
                    completedTasks = completedTasks.OrderBy(t => t.Title);
                    break;
            }

            var sortedCompletedTasks = await completedTasks.ToListAsync();

            return View(sortedCompletedTasks);
        }

        [Authorize]
        public async Task<IActionResult> PendingTasks(string sortOrder)
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);

            // Retrieve pending tasks for the user from the database
            var pendingTasks = _context.tasks.Where(t => !t.IsCompleted && t.UserId == user.Id);

            switch (sortOrder)
            {
                case "Title":
                    pendingTasks = pendingTasks.OrderBy(t => t.Title);
                    break;
                case "DueDate":
                    pendingTasks = pendingTasks.OrderBy(t => t.DueDate);
                    break;
                case "Priority":
                    pendingTasks = pendingTasks.OrderBy(t => t.Priority);
                    break;
                default:
                    pendingTasks = pendingTasks.OrderBy(t => t.Title);
                    break;
            }

            var sortedPendingTasks = await pendingTasks.ToListAsync();

            return View(sortedPendingTasks);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchTask(string searchTerm)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var searchTasks = _context.tasks.Where(t => t.UserId == user.Id);

            // Apply filtering based on searchTerm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTasks = searchTasks.Where(t => t.Title.Contains(searchTerm) || t.Description.Contains(searchTerm));
            }

            var sortedAndFilteredTasks = await searchTasks.ToListAsync();

            return View(sortedAndFilteredTasks);
        }

        [Authorize]
        public IActionResult Create() 
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,DueDate,Category,Priority,IsCompleted")] Tasks tasks)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }
            tasks.UserId = currentUser.Id;
            tasks.DateCreated = DateTime.UtcNow;
            _context.Add(tasks);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        // GET: Tasks/Edit/
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        [Authorize]
        // POST: Tasks/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,DueDate,Category,Priority,IsCompleted")] Tasks task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Set the UserId for the task object
            task.UserId = userId;

            _context.tasks.Update(task);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            //return View(task);
        }

        [Authorize]
        public IActionResult Delete(int id)
        {
            var item = _context.tasks.FirstOrDefault(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Delete(Tasks item)
        {
            _context.tasks.Remove(item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}