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
using Microsoft.Build.Framework;
using SkiaSharp;
using Microsoft.AspNetCore.Components;


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

        //Methods for: Submissions.razor
        public async Task<Submission> CreateNewSubmissionAsync(int renewalId, int? carrierId = null, int? wholesalerId = null)
        {
            Submission submission = new Submission();

            if (carrierId.HasValue)
            {
                var carrier = await _context.Carriers.FirstOrDefaultAsync(c => c.CarrierId == carrierId.Value);
                submission.Carrier = carrier;
            }

            if (wholesalerId.HasValue)
            {
                var wholesaler = await _context.Carriers.FirstOrDefaultAsync(c => c.CarrierId == wholesalerId.Value);
                submission.Wholesaler = wholesaler;
            }

            

            var renewal = await _context.Renewals.FirstOrDefaultAsync(c => c.RenewalId == renewalId);
            submission.Renewal = renewal;
            submission.SubmissionStatus = "Started";
            submission.Product = renewal.Product;
            submission.SubmissionDate = DateTime.Now;
            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync();

            return submission;
        }

        public async Task UpdateSubmissionStatusAsync(int submissionStatusId, Submission submission)
        {
            submission.StatusInt = submissionStatusId;
            await _context.SaveChangesAsync();   
        }

        public async Task UpdateNotesAndPremiumAsync(Submission submission)
        {
            await _context.SaveChangesAsync();
        }

        //Method for: View.razor
        public async Task<Renewal> GetRenewalByIdAsync(int renewalId)
        {
            var renewalRecord = await _context.Renewals
                    .Include(r => r.Carrier)
                    .Include(r => r.Wholesaler)
                    .Include(r => r.Client)
                    .Include(r => r.Product)
                    .Include(r => r.AssignedTo)
                    .Include(r => r.Submissions)
                        .ThenInclude(s => s.Carrier)
                            .ThenInclude(c => c.Contacts)
                    .Include(r => r.Submissions)
                        .ThenInclude(s => s.Wholesaler)
                            .ThenInclude(w => w.Contacts)
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

        public async Task UpdateRenewal(Renewal renewal)
        { 
            Console.WriteLine("Updating r    INSIDE   enewal");
            _context.Entry(renewal).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            Console.WriteLine("Done");
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

        public async Task UpdateTaskHidden(int taskItemId, bool isHidden)
        {
            var task = await _context.TrackTasks.FindAsync(taskItemId);
            if (task != null)
            {
                task.Hidden = isHidden;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ApplicationUser> AssignToMe(int taskItemId)
        {
            var task = await _context.TrackTasks.FindAsync(taskItemId);
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            task.AssignedTo = currentUser;
            await _context.SaveChangesAsync();
            return currentUser;
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

        //Methods for: Home.razor
        public List<Renewal> GetFilteredRenewalList(int? myMonth, int? myYear, string? myUserId)
        {
            // Default to the current month and year if they are null
            int month = myMonth ?? DateTime.Now.Month;
            int year = myYear ?? DateTime.Now.Year;

            var renewals = _context.Renewals
                .Where(r => r.RenewalDate.Month == month && r.RenewalDate.Year == myYear)
                .Include(r => r.Product)
                .Include(r => r.Client)
                .Include(r => r.Carrier)
                .Include(r => r.Wholesaler)
                .Include(r => r.Policy)
                .Include(r => r.AssignedTo)
                .ToList();

            if (myUserId != null)
            {
                if (myUserId != "Everyone")
                {
                    renewals = renewals.Where(r => r.AssignedToId == myUserId).ToList();
                }
            }

            // Execute the query and return the filtered list
            return renewals;
        }

        public async Task<List<Renewal>> GetFilteredRenewalListAsync(int? myMonth, int? myYear, string? myUserId)
        {
            int month = myMonth ?? DateTime.Now.Month;
            int year = myYear ?? DateTime.Now.Year;

            IQueryable<Renewal> renewalsQuery = _context.Renewals
                .Where(r => r.RenewalDate.Month == month && r.RenewalDate.Year == year)
                .Include(r => r.Product)
                .Include(r => r.Client)
                .Include(r => r.Carrier)
                .Include(r => r.Wholesaler)
                .Include(r => r.Policy)
                .Include(r => r.AssignedTo)
                .Include(r => r.TrackTasks)
                .OrderBy(r => r.RenewalDate);

            if (myUserId != null && myUserId != "Everyone")
            {
                renewalsQuery = renewalsQuery.Where(r => r.AssignedToId == myUserId);
            }

            return await renewalsQuery.ToListAsync();
        }


        public async Task<List<Policy>> GetFilteredRenewalOrphanListAsync(int? myMonth, int? myYear)
        {
            int month = myMonth ?? DateTime.Now.Month;
            int year = myYear ?? DateTime.Now.Year;

            IQueryable<Policy> policyQuery = _context.Policies
                .Where(p => p.ExpirationDate.Month == month && p.ExpirationDate.Year == year)
                .Where(p => !_context.Renewals.Any(r => r.PolicyId == p.PolicyId))  // Exclude policies that already have a renewal
                .Include(p => p.Product)
                .Include(p => p.Client)
                .Include(p => p.Carrier)
                .Include(p => p.Wholesaler)
                .OrderBy(p => p.ExpirationDate);

            return await policyQuery.ToListAsync();
        }


        //Methods for: Create.razor List.razor
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

        public async Task<Renewal> CreateRenewalFromPolicyOld(int policyid)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

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
                RenewalDate = policy.ExpirationDate,
                Policy = policy
            };

            if (policy.Product == null)
            {
                if (policy.eType.Contains("professional", StringComparison.OrdinalIgnoreCase))
                {
                    renewal.Product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 5);
                }
                else if (policy.eType.Contains("general", StringComparison.OrdinalIgnoreCase))
                {
                    renewal.Product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 3);
                }
                else if (policy.eType.Contains("work", StringComparison.OrdinalIgnoreCase))
                {
                    renewal.Product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 2);
                }
                else if (policy.eType.Contains("auto", StringComparison.OrdinalIgnoreCase))
                {
                    renewal.Product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 4);
                }
                else if (policy.eType.Contains("business", StringComparison.OrdinalIgnoreCase) || policy.eType.Contains("bop"))
                {
                    renewal.Product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 6);
                }
                else if (policy.eType.Contains("umb", StringComparison.OrdinalIgnoreCase))
                {
                    renewal.Product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 7);
                }
                else if (policy.eType.Contains("practice", StringComparison.OrdinalIgnoreCase) || policy.eType.Contains("epli") || policy.eTypeCode.Contains("epli"))
                {
                    renewal.Product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 8);
                }
                else if (policy.eType.Contains("med", StringComparison.OrdinalIgnoreCase))
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
        public async Task<int?> GetExistingRenewalIdAsync(int policyId)
        {
            // Check if a renewal exists for the given policy ID and return the RenewalId if it does
            var existingRenewalId = await _context.Renewals
                .Where(r => r.Policy.PolicyId == policyId)
                .Select(r => r.RenewalId)
                .FirstOrDefaultAsync();

            // If no renewal exists, return null
            return existingRenewalId == 0 ? (int?)null : existingRenewalId;
        }

        //This has been generating concurrent context errors, using CreateRenewalFromPolicyAsync instead
        public async Task<Renewal> CreateRenewalFromPolicy(int policyid)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var policy = await _context.Policies
                .Include(p => p.Client)
                .Include(p => p.Carrier)
                .Include(p => p.Wholesaler)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(p => p.PolicyId == policyid);

            if (policy == null) throw new Exception("Policy not found");

            Renewal renewal = new Renewal
            {
                ExpiringPolicyNumber = policy.PolicyNumber,
                Wholesaler = policy.Wholesaler,
                Carrier = policy.Carrier,
                Client = policy.Client,
                ExpiringPremium = policy.Premium,
                RenewalDate = policy.ExpirationDate,
                Policy = policy,
                Product = await GetProductForPolicyAsync(policy)
            };

            
            renewal.AssignedTo = currentUser;

            _context.Renewals.Add(renewal);
            _context.SaveChanges();

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

        public async Task<Renewal> CreateRenewalFromPolicyAsync(int policyId)
        {
            // Retrieve the current user
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            // Retrieve the policy with related entities
            var policy = await _context.Policies
                .Include(p => p.Client)
                .Include(p => p.Carrier)
                .Include(p => p.Wholesaler)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(p => p.PolicyId == policyId);

            if (policy == null) throw new Exception("Policy not found");

            // Create a new renewal
            var renewal = new Renewal
            {
                ExpiringPolicyNumber = policy.PolicyNumber,
                Wholesaler = policy.Wholesaler,
                Carrier = policy.Carrier,
                Client = policy.Client,
                ExpiringPremium = policy.Premium,
                RenewalDate = policy.ExpirationDate,
                Policy = policy,
                Product = await GetProductForPolicyAsync(policy),
                AssignedTo = currentUser
            };

            _context.Renewals.Add(renewal);
            await _context.SaveChangesAsync();

            // Load task masters and create associated tasks
            var taskMasters = await _context.TaskMasters.ToListAsync();
            foreach (var taskMaster in taskMasters)
            {
                var goalDate = taskMaster.DaysBeforeExpiration.HasValue
                    ? renewal.RenewalDate.AddDays(-taskMaster.DaysBeforeExpiration.Value)
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


        private async Task<Product> GetProductForPolicyAsync(Policy policy)
        {
            if (policy.Product != null) return policy.Product;

            return policy.eType.ToLower() switch
            {
                var e when e.Contains("professional") => await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 5),
                var e when e.Contains("general") => await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 3),
                var e when e.Contains("work") => await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 2),
                var e when e.Contains("auto") => await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 4),
                var e when e.Contains("business") || e.Contains("bop") => await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 6),
                var e when e.Contains("umb") => await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 7),
                var e when e.Contains("practice") || e.Contains("epli") || policy.eTypeCode.Contains("epli") => await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 8),
                var e when e.Contains("med") => await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 9),
                _ => await _context.Products.FirstOrDefaultAsync(p => p.ProductId == 10),
            };
        }

        //Seperated Create and Editor pages
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

        //Not used?
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
        public async Task<List<Submission>> GetSubmissionsByRenewalId(int renewalId)
        {
            var submissions = await _context.Submissions
                .Where(s => s.Renewal.RenewalId == renewalId)
                .Include(s => s.Carrier)
                    .ThenInclude(c => c.Contacts)
                .Include(s => s.Wholesaler)
                    .ThenInclude(w => w.Contacts)
                .Include(s => s.Renewal)
                .Include(s => s.Product)
                .ToListAsync();

            return submissions;
        }
        
    }

}
