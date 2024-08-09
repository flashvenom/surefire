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
using Mantis.Domain.Policies.Models;


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



        public async Task<RenewalEditViewModel> GetRenewalEditViewModelByIdAsync(int renewalId)
        {
            var renewal = await _context.Renewals
                .Where(r => r.RenewalId == renewalId)
                .Select(r => new RenewalEditViewModel
                {
                    RenewalId = r.RenewalId,
                    PolicyNumber = r.ExpiringPolicyNumber,
                    ExpiringPremium = r.ExpiringPremium,
                    RenewalDate = r.RenewalDate,
                    AssignedToId = r.AssignedToId,
                    ProductId = r.Product.ProductId,
                    CarrierId = r.Carrier.CarrierId,
                    WholesalerId = r.Wholesaler.CarrierId
                })
                .FirstOrDefaultAsync();

            return renewal;
        }

        // Update the Renewal record
        public async Task UpdateRenewalAsync(RenewalEditViewModel model)
        {
            var renewal = await _context.Renewals.FindAsync(model.RenewalId);

            if (renewal != null)
            {
                //renewal.Policy.PolicyNumber = model.PolicyNumber;
                //renewal.ExpiringPremium = model.ExpiringPremium;
                //renewal.RenewalDate = model.RenewalDate;
                //renewal.AssignedToId = model.AssignedToId;
                renewal.Product.ProductId = model.ProductId;
                //renewal.Carrier.CarrierId = model.CarrierId;
                //renewal.Wholesaler.CarrierId = model.WholesalerId;
                //renewal.Client.ClientId = model.ClientId;

                _context.Renewals.Update(renewal);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Renewal>> GetAllRenewalsAsync()
        {
            var renewals = await _context.Renewals
                .Include(p => p.Client)
                .Include(p => p.AssignedTo)
                .Include(p => p.Policy)
                .Include(p => p.Carrier)
                .Include(p => p.Wholesaler)
                .Include(p => p.Product)
                .ToListAsync();
            return renewals;
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

        public async Task<Renewal> CreateRenewalFromPolicy(int policyid)
        {
            var policy = await _context.Policies
                .Include(p => p.Client)
                .Include(p => p.Carrier)
                .Include(p => p.Wholesaler)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(p => p.PolicyId == policyid);

            Renewal renewal = new Renewal
            {
                ExpiringPolicyNumber = policy.PolicyNumber,
                Wholesaler = policy.Wholesaler,
                Carrier = policy.Carrier,
                Client = policy.Client,
                ExpiringPremium = policy.Premium,
                RenewalDate = policy.ExpirationDate
            };

            if (policy.Product == null)
            {
                if (policy.eType.Contains("professional"))
                {
                    renewal.Product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 5);
                }else if (policy.eType.Contains("general"))
                {
                    renewal.Product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 3);
                }else if (policy.eType.Contains("work"))
                {
                    renewal.Product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 2);
                }else if (policy.eType.Contains("auto"))
                {
                    renewal.Product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 4);
                }else if (policy.eType.Contains("business") || policy.eType.Contains("bop"))
                {
                    renewal.Product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 6);
                }
                else if (policy.eType.Contains("umb"))
                {
                    renewal.Product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 7);
                }
                else if (policy.eType.Contains("practice") || policy.eType.Contains("epli") || policy.eTypeCode.Contains("epli"))
                {
                    renewal.Product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 8);
                }
                else if (policy.eType.Contains("med"))
                {
                    renewal.Product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 9);
                }
                else
                {
                    renewal.Product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 10);
                }
            }
            else
            {
                renewal.Product = policy.Product;
            }

            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            renewal.AssignedTo = currentUser;

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
            return renewal;
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
        public async Task UpdateTaskNotesAsync(int taskItemId, string newNotes)
        {
            var task = await _context.TrackTasks.FirstOrDefaultAsync(t => t.Id == taskItemId);
            if (task != null)
            {
                task.Notes = newNotes;
                _context.TrackTasks.Update(task);
                await _context.SaveChangesAsync();
            }
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
        public async Task<List<Client>> GetClientsAsync()
        {
            var clients = await _context.Clients.ToListAsync();
            return clients;
        }
        //END------------------------

    }

}
