using Microsoft.AspNetCore.Mvc;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Mantis.Data;
using Mantis.Domain.Renewals.Models;
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
                    TaskName = t.TaskName,
                    Completed = t.Completed,
                    Highlighted = t.Highlighted,
                    Hidden = t.Hidden,
                    Important = t.Important,
                    Status = t.Status,
                    Notes = t.Notes
                }).ToListAsync();

            return tasks;
        }


    }

}
