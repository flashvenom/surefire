using Surefire.Data;
using Surefire.Domain.Renewals.ViewModels;
using Surefire.Domain.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Surefire.Domain.Shared.Services;
using Surefire.Domain.Renewals.Models;

namespace Surefire.Domain.Renewals.Services
{
    public class TaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly StateService _stateService;
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public TaskService(StateService stateService, ApplicationDbContext context, IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _stateService = stateService;
            _context = context;
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<HomePageTasksViewModel>> GetIncompleteTasks()
        {
            using var context = _dbContextFactory.CreateDbContext();

            var today = DateTime.Today;
            var cutoffDate = today.AddDays(7);
            var currentUser = _stateService.CurrentUser;

            // Fetch all incomplete tasks assigned to the current user where RenewalDate is today+7 days or older
            var tasks = await context.TrackTasks
                .Include(t => t.Renewal)
                    .ThenInclude(r => r.Policy)
                        .ThenInclude(s => s.Product)
                .Include(t => t.Renewal)
                    .ThenInclude(r => r.Client)
                .Where(t => t.Completed == false) // Only incomplete tasks
                .Where(t => t.AssignedTo == currentUser || t.Renewal.AssignedTo == currentUser) // Assigned to current user
                .Where(t => t.Renewal.RenewalDate <= cutoffDate) // RenewalDate is today + 7 days or older
                .OrderBy(t => t.Renewal.RenewalDate) // Order by RenewalDate with oldest at top
                .Take(20) // Limit to 20 tasks
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
                PolicyProduct = t.Renewal.Policy?.Product?.LineCode ?? "Unknown",
                RenewalDate = t.Renewal.RenewalDate,
                Priority = $"Renewal Date: {t.Renewal.RenewalDate.ToShortDateString()}"
            }).ToList();

            return result;
        }

        public async Task<List<DailyTask>> UpdateDailyTaskAsync(DailyTask task)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var existingTask = await context.DailyTasks.FindAsync(task.Id);
            if (existingTask != null)
            {
                existingTask.Completed = task.Completed;
                if (task.Completed)
                {
                    existingTask.CompletedDate = DateTime.Now;
                }
                await context.SaveChangesAsync();
            }
            return await GetDailyTasksAsync();
        }

        public async Task<List<DailyTask>> AddNewDailyTaskAsync(DailyTask task)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var currentUser = _stateService.CurrentUser;
            context.Attach(currentUser);
            task.AssignedTo = currentUser; // Set the foreign key directly
            task.DateCreated = DateTime.Now;
            task.Order = 100;
            context.DailyTasks.Add(task);
            await context.SaveChangesAsync();
            return await GetDailyTasksAsync();
        }

        public async Task<List<DailyTask>> GetDailyTasksAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var currentUser = _stateService.CurrentUser;
            var tasks = await context.DailyTasks
                .Where(task => !task.Completed)
                .Where(t => t.AssignedTo == currentUser)
                .OrderByDescending(t => t.DateCreated)
                .ToListAsync();
            return tasks;
        }

        public async Task UpdateDailyTaskOrderAsync(List<DailyTask> tasks)
        {
            using var context = _dbContextFactory.CreateDbContext();
            foreach (var task in tasks)
            {
                context.DailyTasks.Update(task);
            }

            await context.SaveChangesAsync();
        }


        // MASTER TASKS -------------------------------------------------------------------------//

        public async Task<List<TaskMaster>> GetAllTaskMastersAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.TaskMasters
                .OrderBy(t => t.OrderNumber)
                .ToListAsync();
        }

        public async Task<TaskMaster> GetTaskMasterByIdAsync(int taskId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.TaskMasters.FindAsync(taskId);
        }

        public async Task<TaskMaster> AddTaskMasterAsync(TaskMaster newTask)
        {
            using var context = _dbContextFactory.CreateDbContext();

            // Assign a default order number if not provided
            if (newTask.OrderNumber == 0)
            {
                newTask.OrderNumber = (await context.TaskMasters.MaxAsync(t => (int?)t.OrderNumber) ?? 0) + 1;
            }

            await context.TaskMasters.AddAsync(newTask);
            await context.SaveChangesAsync();
            return newTask;
        }

        public async Task<TaskMaster> UpdateTaskMasterAsync(TaskMaster updatedTask)
        {
            using var context = _dbContextFactory.CreateDbContext();

            var existingTask = await context.TaskMasters.FindAsync(updatedTask.Id);
            if (existingTask == null)
            {
                throw new InvalidOperationException("TaskMaster not found.");
            }

            existingTask.TaskName = updatedTask.TaskName;
            existingTask.Description = updatedTask.Description;
            existingTask.DaysBeforeExpiration = updatedTask.DaysBeforeExpiration;
            existingTask.ParentTaskId = updatedTask.ParentTaskId;
            existingTask.ForType = updatedTask.ForType;
            existingTask.Important = updatedTask.Important;
            existingTask.OrderNumber = updatedTask.OrderNumber;

            await context.SaveChangesAsync();
            return existingTask;
        }

        public async Task DeleteTaskMasterAsync(int taskId)
        {
            using var context = _dbContextFactory.CreateDbContext();

            var task = await context.TaskMasters.FindAsync(taskId);
            if (task == null)
            {
                throw new InvalidOperationException("TaskMaster not found.");
            }

            context.TaskMasters.Remove(task);
            await context.SaveChangesAsync();
        }

    }
}
