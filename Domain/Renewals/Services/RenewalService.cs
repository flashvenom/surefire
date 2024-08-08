using Microsoft.AspNetCore.Mvc;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Mantis.Data;
using Mantis.Domain.Renewals.Models;
using Mantis.Domain.Renewals.ViewModels;
using Mantis.Domain.Carriers.Models;
using Mantis.Domain.Clients.Models;
using Mantis.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Mantis.Domain.Renewals.ViewModels;


namespace Mantis.Domain.Renewals.Services
{
    public class RenewalService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RenewalService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        //Fill the objects here now, but use the appropriate methods from the Domain classes ASAP
        public async Task<List<Carrier>> GetCarriersAsync()
        {
            return await _context.Carriers.ToListAsync();
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var products = await _context.Products.ToListAsync();
            return products;
        }

        public async Task<List<ApplicationUser>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<List<Client>> GetClientsAsync()
        {
            var clients = await _context.Clients.ToListAsync();
            return clients;
        }

        public async Task NewRenewalAsync(Renewal renewal)
        {
            _context.Renewals.Add(renewal);

            var taskMasters = await _context.TaskMasters.ToListAsync();
            foreach (var taskMaster in taskMasters)
            {
                var goalDate = taskMaster.DaysBeforeExpiration.HasValue
                    ? renewal.RenewalDate.AddDays(-(taskMaster.DaysBeforeExpiration.Value))
                    : (DateTime?)null;

                var trackTask = new TrackTask
                {
                    Renewal = renewal,
                    OrderNumber = taskMaster.OrderNumber,
                    TaskName = taskMaster.TaskName,
                    GoalDate = goalDate,
                    Status = "Pending",
                    Completed = false,
                    Hidden = false,
                    Notes = taskMaster.Description
                };
                _context.TrackTasks.Add(trackTask);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<Renewal> GetRenewalByIdAsync(int renewalId)
        {
            var renewalRecord = await _context.Renewals
                    .Include(r => r.Carrier)
                    .Include(r => r.Wholesaler)
                    .Include(r => r.Client)
                    .Include(r => r.Product)
                    .Include(r => r.AssignedTo)
                    .Include(r => r.TrackTasks)
                    .FirstOrDefaultAsync(r => r.RenewalId == renewalId);

            if (renewalRecord != null)
            {
                return renewalRecord;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<TaskItemViewModel>> GetTasksForRenewalAsync(int renewalId)
        {
            var tasks = await _context.TrackTasks
                .Where(t => t.Renewal.RenewalId == renewalId)
                .Select(t => new TaskItemViewModel
                {
                    TaskItemId = t.Id,
                    TaskItemName = t.TaskName,
                    IsCompleted = t.Completed,
                    IsHighlighted = t.Highlighted,
                    IsHidden = t.Hidden,
                    Status = t.Status,
                    TaskGoalDate = t.GoalDate,
                    TaskCompletedDate = t.CompletedDate,
                    AssignedSubUser = t.AssignedTo,
                    Notes = t.Notes
                }).ToListAsync();

            return tasks;
        }

        public async Task UpdateTaskCompleted(int taskItemId, bool isCompleted)
        {
            var task = await _context.TrackTasks.FindAsync(taskItemId);
            if (task != null)
            {
                task.Completed = isCompleted;
                task.CompletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateTaskHighlight(int taskItemId, bool isHighlighted)
        {
            var task = await _context.TrackTasks.FindAsync(taskItemId);
            if (task != null)
            {
                task.Highlighted = isHighlighted;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<TrackTaskEditViewModel> GetTrackTaskByIdAsync(int taskId)
        {
            var task = await _context.TrackTasks
                .Include(t => t.AssignedTo)
                .Include(t => t.Renewal)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null) return null;

            var users = await _context.Users.ToListAsync();

            return new TrackTaskEditViewModel
            {
                Id = task.Id,
                TaskName = task.TaskName,
                Status = task.Status,
                Completed = task.Completed,
                Hidden = task.Hidden,
                Highlighted = task.Highlighted,
                Notes = task.Notes,
                GoalDate = task.GoalDate,
                CompletedDate = task.CompletedDate,
                Renewal = task.Renewal,
                UserName = task.AssignedTo?.UserName,
                Users = users
            };
        }

        public async Task UpdateTrackTaskAsync(TrackTaskEditViewModel model)
        {
            var task = await _context.TrackTasks.FindAsync(model.Id);
            if (task != null)
            {
                task.TaskName = model.TaskName;
                task.Status = model.Status;
                task.Completed = model.Completed;
                task.Hidden = model.Hidden;
                task.Highlighted = model.Highlighted;
                task.Notes = model.Notes;
                task.GoalDate = model.GoalDate;
                task.CompletedDate = model.CompletedDate;

                if (!string.IsNullOrEmpty(model.UserName))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);
                    task.AssignedTo = user;
                }
                else
                {
                    task.AssignedTo = null;
                }

                await _context.SaveChangesAsync();
            }
        }



    }

}
