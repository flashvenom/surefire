using Mantis.Data;
using Mantis.Domain.Renewals.ViewModels;
using Mantis.Domain.Shared;
using Mantis.Domain.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mantis.Domain.Renewals.Services
{
    public class TaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TaskService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<HomePageTasksViewModel>> GetHomePageTasksAsync()
        {
            var today = DateTime.Today;
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            // Fetch all TrackTasks from the database
            var tasks = await _context.TrackTasks
                .Include(t => t.Renewal)
                    .ThenInclude(r => r.Policy)
                        .ThenInclude(s => s.Product)
                .Include(t => t.Renewal)
                    .ThenInclude(r => r.Client)
                .Where(t => t.Completed == false)
                .Where(t => t.GoalDate != null || t.Highlighted == true)
                .Where(t => t.AssignedTo == currentUser || t.Renewal.AssignedTo == currentUser)
                .ToListAsync();

            // Create the ViewModel list
            var result = tasks.Select(t => new HomePageTasksViewModel
            {
                RenewalId = t.Renewal.RenewalId,
                TaskName = t.TaskName,
                TaskNote = t.Notes,
                Highlighted = t.Highlighted,
                GoalDate = t.GoalDate,
                ClientName = t.Renewal.Client.Name,
                ClientId = t.Renewal.Client.ClientId,
                PolicyProduct = t.Renewal.Policy?.Product.LineCode,
                RenewalDate = t.Renewal.RenewalDate,
                Priority = t.GoalDate.HasValue
                    ? (t.GoalDate < today
                        ? $"<span style='color:red'>{(today - t.GoalDate.Value).Days} Days Late</span>"
                        : $"{(t.GoalDate.Value - today).Days} Days Left")
                    : "ASAP"
            }).ToList();

            // Order by IsHighlighted, Past Due, and Next 20 upcoming tasks
            return result
                .OrderByDescending(t => t.Highlighted)
                .ThenByDescending(t => t.GoalDate.HasValue && t.GoalDate < today)
                .ThenBy(t => t.GoalDate)
                .Take(20)
                .ToList();
        }

        public async Task<List<HomePageTasksViewModel>> GetIncompleteTasks()
        {
            var today = DateTime.Today;
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            // Fetch all TrackTasks from the database
            var tasks = await _context.TrackTasks
                .Include(t => t.Renewal)
                    .ThenInclude(r => r.Policy)
                        .ThenInclude(s => s.Product)
                .Include(t => t.Renewal)
                    .ThenInclude(r => r.Client)
                .Where(t => t.Completed == false)
                .Where(t => t.GoalDate != null || t.Highlighted == true)
                .Where(t => t.AssignedTo == currentUser || t.Renewal.AssignedTo == currentUser)
                .ToListAsync();

            // Create the ViewModel list
            var result = tasks.Select(t => new HomePageTasksViewModel
            {
                RenewalId = t.Renewal.RenewalId,
                TaskName = t.TaskName,
                TaskNote = t.Notes,
                Highlighted = t.Highlighted,
                GoalDate = t.GoalDate,
                ClientName = t.Renewal.Client.Name,
                ClientId = t.Renewal.Client.ClientId,
                PolicyProduct = t.Renewal.Policy?.Product.LineCode,
                RenewalDate = t.Renewal.RenewalDate,
                Priority = t.GoalDate.HasValue
                    ? (t.GoalDate < today
                        ? $"<span style='color:red'>{(today - t.GoalDate.Value).Days} Days Past Due</span>"
                        : $"{(t.GoalDate.Value - today).Days} Days Left")
                    : "ASAP"
            }).ToList();

            // Order by IsHighlighted, Past Due, and Next 20 upcoming tasks
            return result
                .OrderByDescending(t => t.Highlighted)
                .ThenByDescending(t => t.GoalDate.HasValue && t.GoalDate < today)
                .ThenBy(t => t.GoalDate)
                .Take(10)
                .ToList();
        }

        public async Task<List<HomePageTasksViewModel>> GetIncompleteTasksForCurrentUserAsync()
        {
            // Get the currently logged-in user
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            // Fetch all incomplete TrackTasks assigned to the current user or sub-assigned
            var tasks = await _context.TrackTasks
                .Include(t => t.Renewal)
                    .ThenInclude(r => r.Policy)
                        .ThenInclude(p => p.Product)
                .Include(t => t.Renewal)
                    .ThenInclude(r => r.Client)
                .Where(t => t.Completed == false)
                .Where(t => t.AssignedTo == currentUser || t.Renewal.AssignedTo == currentUser)
                .OrderBy(t => t.GoalDate ?? t.Renewal.RenewalDate) // Order by GoalDate or RenewalDate if GoalDate is null
                .Take(30)
                .ToListAsync();

            // Create the ViewModel list
            var result = tasks.Select(t => new HomePageTasksViewModel
            {
                RenewalId = t.Renewal.RenewalId,
                TaskName = t.TaskName,
                TaskNote = t.Notes,
                Highlighted = t.Highlighted,
                GoalDate = t.GoalDate,
                ClientName = t.Renewal.Client.Name,
                ClientId = t.Renewal.Client.ClientId,
                PolicyProduct = t.Renewal.Policy?.Product.LineCode,
                RenewalDate = t.Renewal.RenewalDate,
                Priority = t.GoalDate.HasValue
                    ? (t.GoalDate < DateTime.Today
                        ? $"<span style='color:red'>{(DateTime.Today - t.GoalDate.Value).Days} Days Past Due</span>"
                        : $"{(t.GoalDate.Value - DateTime.Today).Days} Days Left")
                    : "ASAP"
            }).ToList();

            return result;
        }


        public async Task<List<DailyTask>> GetDailyTasksAsync()
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            var tasks = await _context.DailyTasks
                .Where(task => !task.Completed)
                .Where(t => t.AssignedTo == currentUser)
                .ToListAsync();
            return tasks;
        }

        public async Task<List<DailyTask>> UpdateDailyTaskAsync(DailyTask task)
        {
            if(task.Completed)
            {
                task.CompletedDate = DateTime.Now;
            }
            _context.DailyTasks.Update(task);

            await _context.SaveChangesAsync();

            return await GetDailyTasksAsync();
        }

        public async Task<List<DailyTask>> AddNewDailyTaskAsync(DailyTask task)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            task.AssignedTo = currentUser;
            task.DateCreated = DateTime.Now;

            _context.DailyTasks.Add(task);

            await _context.SaveChangesAsync();

            return await GetDailyTasksAsync();
        }


    }
}
