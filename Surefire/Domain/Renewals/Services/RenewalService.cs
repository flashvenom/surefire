using System.Data;
using Surefire.Data;
using Surefire.Domain.Renewals.Models;
using Surefire.Domain.Renewals.ViewModels;
using Surefire.Domain.Carriers.Models;
using Surefire.Domain.Clients.Models;
using Surefire.Domain.Shared.Models;
using Surefire.Domain.Shared.Services;
using Surefire.Domain.Policies.Models;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor.Data;

namespace Surefire.Domain.Renewals.Services
{
    public class RenewalService
    {
        private readonly StateService _stateService;
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public RenewalService(StateService stateService, IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _stateService = stateService;
            _dbContextFactory = dbContextFactory;
        }

        // RENEWALS [GET]-----------------------------------------------------------------//
        public async Task<List<Renewal>> GetAllRenewals()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var myrenewals = await context.Renewals
                .Include(r => r.Client)
                .Include(r => r.Product)
                .Include(r => r.Policy)
                .Include(r => r.Carrier)
                .Include(r => r.Wholesaler)
                .Select(r => new Renewal
                {
                    RenewalId = r.RenewalId,
                    ExpiringPolicyNumber = r.ExpiringPolicyNumber ?? "-", // Replace null with "-"
                    ExpiringPremium = r.ExpiringPremium, // Assuming ExpiringPremium is not nullable
                    RenewalDate = r.RenewalDate,
                    // Handle null values for related entities
                    Client = new Client
                    {
                        Name = r.Client != null ? r.Client.Name : "-" // Replace null with "-"
                    },
                    Product = new Product
                    {
                        LineNickname = r.Product != null ? r.Product.LineNickname : "-" // Replace null with "-"
                    },
                    Policy = new Policy
                    {
                        PolicyNumber = r.Policy != null ? r.Policy.PolicyNumber : "-" // Replace null with "-"
                    },
                    Carrier = new Carrier
                    {
                        CarrierName = r.Carrier != null ? r.Carrier.CarrierName : "-" // Replace null with "-"
                    },
                    Wholesaler = new Carrier
                    {
                        CarrierName = r.Wholesaler != null ? r.Wholesaler.CarrierName : "-" // Replace null with "-"
                    }
                })
                .ToListAsync();
            return myrenewals;
        }
        public async Task<Renewal> GetRenewalByIdAsync(int renewalId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var renewalRecord = await context.Renewals
                    .Include(r => r.Carrier)
                    .Include(r => r.Wholesaler)
                    .Include(r => r.Client)
                        .ThenInclude(s => s.Contacts)
                    .Include(r => r.Product)
                    .Include(r => r.AssignedTo)
                    .Include(r => r.Policy)
                    .Include(r => r.Submissions)
                        .ThenInclude(s => s.Carrier)
                            .ThenInclude(c => c.Contacts)
                    .Include(r => r.Submissions)
                        .ThenInclude(s => s.Wholesaler)
                            .ThenInclude(w => w.Contacts)
                    .Include(r => r.Submissions)
                        .ThenInclude(s => s.SubmissionNotes)
                    .Include(r => r.Attachments)
                    .AsSplitQuery()
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
        public async Task<Renewal> GetRenewalByIdTrackAsync(int renewalId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var renewalRecord = await context.Renewals
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
                .AsSplitQuery()
                .FirstOrDefaultAsync(r => r.RenewalId == renewalId);

            return renewalRecord;
        }
        public async Task<List<RenewalListItemViewModel>> GetFilteredRenewalListAsync(int? myMonth, int? myYear, string? myUserId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            int month = myMonth ?? DateTime.Now.Month;
            int year = myYear ?? DateTime.Now.Year;

            // Create the query and select only the required fields
            IQueryable<RenewalListItemViewModel> renewalsQuery = context.Renewals
                .Where(r => r.RenewalDate.Month == month && r.RenewalDate.Year == year)
                .Select(r => new RenewalListItemViewModel
                {
                    RenewalId = r.RenewalId,
                    RenewalDate = r.RenewalDate,
                    ProductLineCode = r.Product.LineCode,
                    ClientName = r.Client.Name,
                    CarrierName = r.Carrier.CarrierName,
                    WholesalerNickname = r.Wholesaler.CarrierNickname,
                    PolicyNumber = r.Policy.PolicyNumber,
                    Premium = r.Policy.Premium,
                    Submits = r.Submissions.Count(),
                    ClientId = r.ClientId,
                    PolicyId = r.Policy.PolicyId,
                    AssignedToFirstName = r.AssignedTo.FirstName,
                    AssignedToLastName = r.AssignedTo.LastName,
                    AssignedToPictureUrl = r.AssignedTo.PictureUrl,

                    TrackTasks = r.TrackTasks,
                    AssignedToId = r.AssignedTo.Id
                })
                .AsNoTracking()
                .OrderBy(r => r.RenewalDate);

            // Apply filter based on myUserId if necessary
            if (myUserId != null && myUserId != "Everyone")
            {
                renewalsQuery = renewalsQuery.Where(r => r.AssignedToId == myUserId);
            }

            // Return the filtered and projected data
            return await renewalsQuery.ToListAsync();
        }
        public async Task<List<Policy>> GetFilteredRenewalOrphanListAsync(int? myMonth, int? myYear)
        {
            int month = myMonth ?? DateTime.Now.Month;
            int year = myYear ?? DateTime.Now.Year;

            using var context = _dbContextFactory.CreateDbContext();
            IQueryable<Policy> policyQuery = context.Policies
                .Where(p => p.ExpirationDate.Month == month && p.ExpirationDate.Year == year)
                .Where(p => !context.Renewals.Any(r => r.PolicyId == p.PolicyId))  // Exclude policies that already have a renewal
                .Include(p => p.Product)
                .Include(p => p.Client)
                .Include(p => p.Carrier)
                .Include(p => p.Wholesaler)
                .AsNoTracking()
                .OrderBy(p => p.ExpirationDate);

            return await policyQuery.ToListAsync();
        }
        public async Task<Renewal> GetRenewalForPurpleSheetAsync(int renewalId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var renewalRecord = await context.Renewals
                    .Include(r => r.Carrier)
                    .Include(r => r.Wholesaler)
                    .Include(r => r.Client)
                    .Include(r => r.Product)
                    .Include(r => r.Policy)
                    .Include(r => r.Submissions)
                    .Include(r => r.Settlements)
                        .ThenInclude(s => s.SettlementItems)
                    .AsSplitQuery()
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

        // RENEWALS [CREATE]-----------------------------------------------------------------//
        public async Task NewRenewalAsync(Renewal renewal)
        {
            using var context = _dbContextFactory.CreateDbContext();
            // Check if a renewal already exists with the same PolicyId
            var existingRenewal = await context.Renewals
                .FirstOrDefaultAsync(r => r.PolicyId == renewal.PolicyId);

            if (existingRenewal != null)
            {
                // Renewal already exists, handle accordingly
                throw new Exception("A renewal for this policy already exists.");
            }
            // Retrieve the current user
            if(renewal.AssignedToId == "me")
            {
                var currentUser = _stateService.CurrentUser;
                context.Attach(currentUser);
                renewal.AssignedTo = currentUser;
            }
            
            renewal.DateCreated = DateTime.Now;

            context.Renewals.Add(renewal);

            var taskMasters = await context.TaskMasters.ToListAsync();
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
                context.TrackTasks.Add(trackTask);
            }
            await context.SaveChangesAsync();
        }
        public async Task<int> CreateRenewalFromPolicyAsync(int policyId)
        {
            using var context = _dbContextFactory.CreateDbContext();

            //Check if the renewal already exists
            var existingRenewalId = await context.Renewals
                .Where(r => r.Policy.PolicyId == policyId)
                .Select(r => r.RenewalId)
                .FirstOrDefaultAsync();

            if (existingRenewalId != 0)
            {
                // Renewal already exists, return the existing RenewalId
                return existingRenewalId;
            }

            // Retrieve the current user
            var currentUser = _stateService.CurrentUser;
            context.Attach(currentUser);

            // Retrieve the policy with related entities
            var policy = await context.Policies
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

            context.Renewals.Add(renewal);
            await context.SaveChangesAsync();

            // Load task masters and create associated tasks
            var taskMasters = await context.TaskMasters.ToListAsync();
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
                context.TrackTasks.Add(trackTask);
            }

            await context.SaveChangesAsync();
            return renewal.RenewalId;
        }
        private async Task<Product> GetProductForPolicyAsync(Policy policy)
        {
            //Used by CreateRenewalFromPolicyAsync to assign the correct product to a renewal being created by a policy
            //This should be reworked since product names may easily change
            using var context = _dbContextFactory.CreateDbContext();

            if (policy.Product != null) return policy.Product;

            var eTypeLower = policy.eType?.ToLower() ?? string.Empty;
            var eTypeCodeLower = policy.eTypeCode?.ToLower() ?? string.Empty;

            return policy.eType.ToLower() switch
            {
                var e when e.Contains("professional") => await context.Products.FirstOrDefaultAsync(p => p.ProductId == 5),
                var e when e.Contains("general") => await context.Products.FirstOrDefaultAsync(p => p.ProductId == 3),
                var e when e.Contains("work") => await context.Products.FirstOrDefaultAsync(p => p.ProductId == 2),
                var e when e.Contains("auto") => await context.Products.FirstOrDefaultAsync(p => p.ProductId == 4),
                var e when e.Contains("business") || e.Contains("bop") => await context.Products.FirstOrDefaultAsync(p => p.ProductId == 6),
                var e when e.Contains("umb") => await context.Products.FirstOrDefaultAsync(p => p.ProductId == 7),
                var e when e.Contains("practice") || e.Contains("epli") || policy.eTypeCode.Contains("epli") => await context.Products.FirstOrDefaultAsync(p => p.ProductId == 8),
                var e when e.Contains("med") => await context.Products.FirstOrDefaultAsync(p => p.ProductId == 9),
                _ => await context.Products.FirstOrDefaultAsync(p => p.ProductId == 10),
            };
        }

        // RENEWALS [UPDATE]-----------------------------------------------------------------//
        public async Task UpdateRenewalAsync(Renewal renewal)
        {
            using var context = _dbContextFactory.CreateDbContext();
            renewal.DateModified = DateTime.Now;
            context.Entry(renewal).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
        public async Task UpdateNotepadAsync(Renewal renewal)
        {
            // Create a new context instance
            using var context = _dbContextFactory.CreateDbContext();

            // Only attach the Renewal entity without any related entities
            var existingRenewal = await context.Renewals.FirstOrDefaultAsync(r => r.RenewalId == renewal.RenewalId);
            if (existingRenewal != null)
            {
                // Update only the Notes field
                existingRenewal.Notes = renewal.Notes;

                // Save changes for only the modified field
                await context.SaveChangesAsync();
            }
        }


        // TASKS [GET] ----------------------------------------------------------------------//
        public async Task<List<TaskItemViewModel>> GetTasksForRenewalAsync(int renewalId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var tasks = await context.TrackTasks
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
        public async Task<TrackTaskEditViewModel> GetTrackTaskByIdAsync(int taskId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.TrackTasks
                .Include(t => t.AssignedTo)
                .Include(t => t.Renewal)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null) return null;

            var users = await context.Users.ToListAsync();

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
        public async Task<List<TaskMaster>> GetAllTaskMasters()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var tasks = await context.TaskMasters.ToListAsync();
            return tasks;
        }

        // TASKS [UPDATE] ----------------------------------------------------------------------//
        public async Task UpdateTaskCompleted(int taskItemId, bool isCompleted)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.TrackTasks.FindAsync(taskItemId);
            if (task != null)
            {
                task.Completed = isCompleted;
                task.CompletedDate = DateTime.Now;
                await context.SaveChangesAsync();
            }
        }
        public async Task UpdateTaskHighlight(int taskItemId, bool isHighlighted)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.TrackTasks.FindAsync(taskItemId);
            if (task != null)
            {
                task.Highlighted = isHighlighted;
                await context.SaveChangesAsync();
            }
        }
        public async Task UpdateTaskHidden(int taskItemId, bool isHidden)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.TrackTasks.FindAsync(taskItemId);
            if (task != null)
            {
                task.Hidden = isHidden;
                if (isHidden == true)
                {
                    task.Completed = true;
                }
                await context.SaveChangesAsync();
            }
        }
        public async Task UpdateTaskNotesAsync(int taskItemId, string newNotes)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.TrackTasks.FirstOrDefaultAsync(t => t.Id == taskItemId);
            if (task != null)
            {
                task.Notes = newNotes;
                context.TrackTasks.Update(task);
                await context.SaveChangesAsync();
            }
        }
        public async Task UpdateTrackTaskAsync(TrackTaskEditViewModel model)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.TrackTasks.FindAsync(model.Id);
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
                task.DateModified = DateTime.Now;

                if (!string.IsNullOrEmpty(model.UserName))
                {
                    var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);
                    task.AssignedTo = user;
                }
                else
                {
                    task.AssignedTo = null;
                }

                await context.SaveChangesAsync();
            }
        }
        public async Task UpdateTrackTaskModelAsync(TaskItemViewModel model)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.TrackTasks.FindAsync(model.TaskItemId);
            if (task != null)
            {
                task.TaskName = model.TaskItemName;
                task.Status = model.Status;
                task.Completed = model.IsCompleted;
                task.Hidden = model.IsHidden;
                task.Highlighted = model.IsHighlighted;
                task.Notes = model.Notes;
                task.GoalDate = model.TaskGoalDate;
                task.CompletedDate = model.TaskCompletedDate;
                task.DateModified = DateTime.Now;

                await context.SaveChangesAsync();
            }
        }
        public async Task<ApplicationUser> AssignToMe(int taskItemId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            

            // Fetch the user from this context
            var userId = _stateService.CurrentUser.Id; // Assuming the ID is available here
            var currentUser = await context.Users.FindAsync(userId);
            if (currentUser == null)
            {
                throw new Exception("User not found in the database.");
            }
            var task = await context.TrackTasks.FindAsync(taskItemId);
            task.AssignedTo = currentUser;
            await context.SaveChangesAsync();
            return currentUser;
        }
        public async Task<ApplicationUser> AssignToSub(int taskItemId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.TrackTasks.FindAsync(taskItemId);
            var subuser = await context.Users.FirstOrDefaultAsync(u => u.Id == "db0723c6-1702-4f55-8ff9-f7128ee68631");
            task.AssignedTo = subuser;
            await context.SaveChangesAsync();
            return subuser;
        }
        public async Task UnassignSub(int taskid)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.TrackTasks.FindAsync(taskid);
            if (task != null)
            {
                task.AssignedTo = null;
                await context.SaveChangesAsync();
            }
        }

        // NOTES -------------------------------------------------------------------------//
        public async Task AddSubmissionNoteAsync(SubmissionNote newNote)
        {
            using var context = _dbContextFactory.CreateDbContext();
            context.SubmissionNotes.Add(newNote);
            await context.SaveChangesAsync();
        }
        public async Task UpdateNotesAndPremiumAsync(Submission submission)
        {
            using var context = _dbContextFactory.CreateDbContext();
            submission.DateModified = DateTime.Now;
            await context.SaveChangesAsync();
        }


        //--------------------------------------------------------------------------------//
        //Why is this even in RenewalService - Move to Interface once we do that
        public async Task<List<Client>> GetClientsAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var clients = await context.Clients.ToListAsync();
            return clients;
        }
        //Create shared state service init stuff for these
        public async Task<List<Product>> GetProductsAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var products = await context.Products.ToListAsync();
            return products;
        }
        
    }
}