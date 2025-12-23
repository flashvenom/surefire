using System.Data;
using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.Renewals.Models;
using Quickfire.Blazor.Domain.Renewals.ViewModels;
using Quickfire.Blazor.Domain.Carriers.Models;
using Quickfire.Blazor.Domain.Clients.Models;
using Quickfire.Blazor.Domain.Shared.Models;
using Quickfire.Blazor.Domain.Shared.Services;
using Quickfire.Blazor.Domain.Attachments.Models;
using Quickfire.Blazor.Domain.Policies.Models;
using Quickfire.Blazor.Domain.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor.Data;
using System.Collections.Generic;

namespace Quickfire.Blazor.Domain.Renewals.Services
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
                    ExpiringPolicyNumber = r.ExpiringPolicyNumber ?? "-",
                    ExpiringPremium = r.ExpiringPremium, // Assuming ExpiringPremium is not nullable
                    RenewalDate = r.RenewalDate,
                    // Handle null values for related entities
                    Client = new Client
                    {
                        Name = r.Client != null ? r.Client.Name : "-"
                    },
                    Product = new Product
                    {
                        LineNickname = r.Product != null ? r.Product.LineNickname : "-"
                    },
                    Policy = new Policy
                    {
                        PolicyNumber = r.Policy != null ? r.Policy.PolicyNumber : "-"
                    },
                    Carrier = new Carrier
                    {
                        CarrierName = r.Carrier != null ? r.Carrier.CarrierName : "-"
                    },
                    Wholesaler = new Carrier
                    {
                        CarrierName = r.Wholesaler != null ? r.Wholesaler.CarrierName : "-"
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
                                .ThenInclude(d => d.EmailAddresses)
                    .Include(r => r.Submissions)
                        .ThenInclude(s => s.Carrier)
                            .ThenInclude(c => c.Contacts)
                                .ThenInclude(d => d.PhoneNumbers)
                    .Include(r => r.Submissions)
                        .ThenInclude(s => s.Wholesaler)
                            .ThenInclude(w => w.Contacts)
                                .ThenInclude(d => d.EmailAddresses)
                    .Include(r => r.Submissions)
                        .ThenInclude(s => s.Wholesaler)
                            .ThenInclude(w => w.Contacts)
                                .ThenInclude(d => d.PhoneNumbers)
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
                    MaxSubmissionStatus = r.Submissions.Any() ? r.Submissions.Max(s => s.StatusInt) : (int?)null,
                    ClientId = r.ClientId,
                    PolicyId = r.Policy.PolicyId,
                    CustomLabel = r.Policy.CustomLabel,
                    AssignedToFirstName = r.AssignedTo.FirstName,
                    AssignedToLastName = r.AssignedTo.LastName,
                    AssignedToPictureUrl = r.AssignedTo.PictureUrl,

                    TrackTasks = r.TrackTasks,
                    AssignedToId = r.AssignedTo.Id,
                    
                    // Warning icon data
                    SettlementBillType = null,
                    RenewalBillType = r.BillType,
                    PolicyBillType = r.Policy.BillType.HasValue ? r.Policy.BillType.ToString() : null,
                    HasNonRenewedSubmission = r.Submissions.Any(s => s.RejectedStatus == RejectedStatus.NonRenewed),
                    RenewalStatus = r.RenewalStatus,
                    PriorityNeeded = r.PriorityNeeded
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

            await EnsureRenewalAttachmentGroupAsync(context, renewal);

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
                var existingRenewal = await context.Renewals.FindAsync(existingRenewalId);
                if (existingRenewal != null && !existingRenewal.AttachmentGroupId.HasValue)
                {
                    await EnsureRenewalAttachmentGroupAsync(context, existingRenewal);
                    await context.SaveChangesAsync();
                }

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
            await EnsureRenewalAttachmentGroupAsync(context, renewal);

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
                    TaskName = taskMaster.TaskName,
                    GoalDate = goalDate,
                    Status = "Pending",
                    Completed = false,
                    Hidden = false
                };
                context.TrackTasks.Add(trackTask);
            }

            await context.SaveChangesAsync();
            return renewal.RenewalId;
        }
        public async Task<int> CreateRenewalWithTaskGroupAsync(int policyId, int taskGroupId, bool reset = false)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();

            // Check if the renewal already exists
            var existingRenewalId = await context.Renewals
                .Where(r => r.PolicyId == policyId)
                .Select(r => r.RenewalId)
                .FirstOrDefaultAsync();

            if (existingRenewalId != 0 && !reset)
            {
                var existingRenewal = await context.Renewals.FindAsync(existingRenewalId);
                if (existingRenewal != null && !existingRenewal.AttachmentGroupId.HasValue)
                {
                    await EnsureRenewalAttachmentGroupAsync(context, existingRenewal);
                    await context.SaveChangesAsync();
                }

                // Renewal already exists, return the existing RenewalId
                return existingRenewalId;
            }

            var currentUser = _stateService.CurrentUser;
            context.Attach(currentUser);

            var policy = await context.Policies
                .Include(p => p.Client)
                .Include(p => p.Carrier)
                .Include(p => p.Wholesaler)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(p => p.PolicyId == policyId);

            if (policy == null) throw new Exception("Policy not found");

            Renewal renewal;
            if (reset && existingRenewalId != 0)
            {
                // Use existing renewal
                renewal = await context.Renewals
                    .Include(r => r.Client)
                    .Include(r => r.Carrier)
                    .Include(r => r.Wholesaler)
                    .Include(r => r.Product)
                    .FirstOrDefaultAsync(r => r.RenewalId == existingRenewalId);

                if (renewal != null && !renewal.AttachmentGroupId.HasValue)
                {
                    await EnsureRenewalAttachmentGroupAsync(context, renewal);
                    await context.SaveChangesAsync();
                }
            }
            else
            {
                // Create a new renewal
                renewal = new Renewal
                {
                    ExpiringPolicyNumber = policy.PolicyNumber,
                    Wholesaler = policy.Wholesaler,
                    Carrier = policy.Carrier,
                    Client = policy.Client,
                    ExpiringPremium = policy.Premium,
                    RenewalDate = policy.ExpirationDate,
                    Policy = policy,
                    Product = policy.Product,
                    AssignedTo = currentUser
                };
                await EnsureRenewalAttachmentGroupAsync(context, renewal);
                context.Renewals.Add(renewal);
                await context.SaveChangesAsync();

                // Add a RenewalNote for creation
                var creationNote = new RenewalNote
                {
                    RenewalId = renewal.RenewalId,
                    Note = "Renewal Created",
                    DateCreated = DateTime.Now,
                    CreatedById = currentUser.Id,
                    NoteType = RenewalNoteType.RenewalUpdate,
                    Deleted = false
                };
                context.RenewalNotes.Add(creationNote);
                await context.SaveChangesAsync();
            }

            // Get the task group with its task masters
            var taskGroup = await context.TaskGroups
                .Include(tg => tg.TaskGroupTaskMasters)
                    .ThenInclude(tgtm => tgtm.TaskMaster)
                .FirstOrDefaultAsync(tg => tg.TaskGroupId == taskGroupId);

            if (taskGroup == null) throw new Exception("Task group not found");

            // Create the tasks in the same order as they appear in the task group
            var createdTrackTasks = new Dictionary<int, TrackTask>(); // TaskMasterId -> TrackTask
            foreach (var taskGroupTaskMaster in taskGroup.TaskGroupTaskMasters.OrderBy(tgtm => tgtm.OrderNumber))
            {
                var taskMaster = taskGroupTaskMaster.TaskMaster;

                var goalDate = taskMaster.DaysBeforeExpiration.HasValue
                    ? renewal.RenewalDate.AddDays(-taskMaster.DaysBeforeExpiration.Value)
                    : (DateTime?)null;

                var trackTask = new TrackTask
                {
                    Renewal = renewal,
                    TaskName = taskMaster.TaskName,
                    GoalDate = goalDate,
                    Status = "Pending",
                    Completed = false,
                    Hidden = false,
                    Notes = taskMaster.Description,
                    AssignedToId = taskMaster.DefaultAssignedToId // assign default staff if set
                };

                context.TrackTasks.Add(trackTask);
                await context.SaveChangesAsync(); // Save to get Id for parent
                createdTrackTasks[taskMaster.TaskMasterId] = trackTask;

                // Find subtasks for this TaskMaster (where this is the parent)
                var subtaskLinks = await context.TaskMasterSubTasks
                    .Where(x => x.ParentTaskMasterId == taskMaster.TaskMasterId)
                    .OrderBy(x => x.OrderNumber)
                    .ToListAsync();

                foreach (var subLink in subtaskLinks)
                {
                    var subTaskMaster = await context.TaskMasters.FindAsync(subLink.SubTaskMasterId);
                    if (subTaskMaster == null) continue;

                    var subGoalDate = subTaskMaster.DaysBeforeExpiration.HasValue
                        ? renewal.RenewalDate.AddDays(-subTaskMaster.DaysBeforeExpiration.Value)
                        : (DateTime?)null;

                    var subTrackTask = new TrackTask
                    {
                        Renewal = renewal,
                        TaskName = subTaskMaster.TaskName,
                        GoalDate = subGoalDate,
                        Status = "Pending",
                        Completed = false,
                        Hidden = false,
                        Notes = subTaskMaster.Description,
                        ParentTaskId = trackTask.Id,
                        AssignedToId = subTaskMaster.DefaultAssignedToId // assign default staff if set
                    };
                    context.TrackTasks.Add(subTrackTask);
                }
                await context.SaveChangesAsync();
            }

            await context.SaveChangesAsync();
            return renewal.RenewalId;
        }
        private static async Task EnsureRenewalAttachmentGroupAsync(ApplicationDbContext context, Renewal renewal, string? nameHint = null)
        {
            if (renewal == null || renewal.AttachmentGroupId.HasValue)
            {
                return;
            }

            var groupName = !string.IsNullOrWhiteSpace(nameHint)
                ? nameHint
                : !string.IsNullOrWhiteSpace(renewal.ExpiringPolicyNumber)
                    ? $"Renewal {renewal.ExpiringPolicyNumber} Attachments"
                    : $"Renewal {renewal.RenewalDate:yyyyMMdd} Attachments";

            var attachmentGroup = new AttachmentGroup
            {
                CreatedAt = DateTime.UtcNow,
                Name = groupName
            };

            context.AttachmentGroups.Add(attachmentGroup);
            await context.SaveChangesAsync();

            renewal.AttachmentGroupId = attachmentGroup.AttachmentGroupId;
        }

        private async Task<Product> GetProductForPolicyAsync(Policy policy)
        {
            //Used by CreateRenewalFromPolicyAsync to assign the correct product to a renewal being created by a policy
            //This should be reworked since product names may easily change
            using var context = _dbContextFactory.CreateDbContext();

            if (policy.Product != null) return policy.Product;

            var eTypeLower = policy.eType?.ToLowerInvariant() ?? string.Empty;
            var eTypeCodeLower = policy.eTypeCode?.ToLowerInvariant() ?? string.Empty;

            var lineCode = eTypeLower switch
            {
                var e when e.Contains("professional") => ProductLineCodes.ProfessionalLiability,
                var e when e.Contains("general") => ProductLineCodes.GeneralLiability,
                var e when e.Contains("work") => ProductLineCodes.WorkersComp,
                var e when e.Contains("auto") => ProductLineCodes.CommercialAuto,
                var e when e.Contains("business") || e.Contains("bop") => ProductLineCodes.BusinessOwners,
                var e when e.Contains("umb") => ProductLineCodes.Umbrella,
                var e when e.Contains("practice") || e.Contains("epli") || eTypeCodeLower.Contains("epli") => ProductLineCodes.EmploymentPractices,
                var e when e.Contains("med") => ProductLineCodes.GroupMedical,
                _ => ProductLineCodes.Other,
            };

            var product = await context.Products.FirstOrDefaultAsync(p => p.LineCode == lineCode);
            if (product != null)
            {
                return product;
            }

            var fallback = await context.Products.FirstOrDefaultAsync(p => p.LineCode == ProductLineCodes.Other);
            return fallback ?? new Product { LineName = "Other", LineNickname = "Other", LineCode = lineCode };
        }

        // RENEWALS [UPDATE]-----------------------------------------------------------------//
        public async Task UpdateRenewalAsync(Renewal renewal)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var existingRenewal = await context.Renewals.FirstOrDefaultAsync(r => r.RenewalId == renewal.RenewalId);

            if (existingRenewal is null)
            {
                throw new InvalidOperationException($"Unable to update renewal {renewal.RenewalId} because it could not be found.");
            }

            existingRenewal.RenewalDate = renewal.RenewalDate;
            existingRenewal.ExpiringPremium = renewal.ExpiringPremium;
            existingRenewal.ExpiringPolicyNumber = renewal.ExpiringPolicyNumber;
            existingRenewal.AssignedToId = renewal.AssignedToId;
            existingRenewal.ProductId = renewal.ProductId;
            existingRenewal.CarrierId = renewal.CarrierId;
            existingRenewal.WholesalerId = renewal.WholesalerId;
            existingRenewal.RenewalStatus = string.IsNullOrWhiteSpace(renewal.RenewalStatus)
                ? "In Progress"
                : renewal.RenewalStatus;
            existingRenewal.BillType = string.IsNullOrWhiteSpace(renewal.BillType)
                ? "Direct Bill"
                : renewal.BillType;
            existingRenewal.PriorityNeeded = renewal.PriorityNeeded;
            existingRenewal.DateModified = DateTime.UtcNow;

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

        // NOTES -------------------------------------------------------------------------//
        // Get all notes for a given submission
        public async Task<List<RenewalNote>> GetNotesForSubmissionAsync(int submissionId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.RenewalNotes
                .Where(n => n.SubmissionId == submissionId && !n.Deleted)
                .OrderByDescending(n => n.DateCreated)
                .ToListAsync();
        }
        // Add a note for a submission using the unified RenewalNote model
        public async Task AddSubmissionNoteAsync(int submissionId, int? renewalId, string noteText, string createdById, RenewalNoteType noteType = RenewalNoteType.SubmissionUserNote)
        {
            using var context = _dbContextFactory.CreateDbContext();

            var note = new RenewalNote
            {
                SubmissionId = submissionId,
                RenewalId = renewalId ?? default(int),
                Note = noteText,
                DateCreated = DateTime.Now,
                CreatedById = createdById,
                Deleted = false,
                NoteType = noteType
            };
            context.RenewalNotes.Add(note);
            await context.SaveChangesAsync();
        }
        public async Task UpdateNotesAndPremiumAsync(Submission submission)
        {
            using var context = _dbContextFactory.CreateDbContext();
            
            // Find the entity in the database
            var existingSubmission = await context.Submissions.FindAsync(submission.SubmissionId);
            if (existingSubmission != null)
            {
                // Update only the properties we want to change
                existingSubmission.StatusInt = submission.StatusInt;
                existingSubmission.Status = submission.Status;
                existingSubmission.Premium = submission.Premium;
                existingSubmission.DateModified = DateTime.Now;
                
                await context.SaveChangesAsync();
            }
        }
        public async Task<List<RenewalNote>> GetRenewalNotesAsync(int renewalId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            List<RenewalNote> myNotes = await context.RenewalNotes
                .Include(n => n.CreatedBy)
                .Where(n => n.RenewalId == renewalId && !n.Deleted)
                .OrderByDescending(n => n.DateCreated)
                .ToListAsync();
           
            return myNotes;
        }
        public async Task AddRenewalNoteAsync(RenewalNote note)
        {
            using var context = _dbContextFactory.CreateDbContext();
            context.RenewalNotes.Add(note);
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
        public async Task CompleteAllTasksAsync(int renewalId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var tasks = await context.TrackTasks
                .Where(t => t.Renewal.RenewalId == renewalId && !t.Completed)
                .ToListAsync();

            foreach (var task in tasks)
            {
                task.Completed = true;
                task.CompletedDate = DateTime.Now;
                task.DateModified = DateTime.Now;
            }

            await context.SaveChangesAsync();
        }

        // TASK NOTES FOR TRACKTASKS ------------------------------------------------------//
        public async Task<List<RenewalNote>> GetRenewalNotesForTrackTaskAsync(int trackTaskId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.RenewalNotes
                .Include(n => n.CreatedBy)
                .Where(n => n.TrackTaskId == trackTaskId && !n.Deleted)
                .OrderByDescending(n => n.DateCreated)
                .ToListAsync();
        }

        public async Task AddRenewalNoteForTrackTaskAsync(int renewalId, int trackTaskId, string noteText, RenewalNoteType noteType = RenewalNoteType.SystemLog)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var note = new RenewalNote
            {
                RenewalId = renewalId,
                TrackTaskId = trackTaskId,
                Note = noteText,
                DateCreated = DateTime.Now,
                CreatedById = _stateService.CurrentUser.Id,
                Deleted = false,
                NoteType = noteType
            };
            context.RenewalNotes.Add(note);
            await context.SaveChangesAsync();
        }

        // AGENT INTEGRATION SUPPORT ---------------------------------------------------//
        public async Task<(int? ClientId, int? ProductId, string? ClientName, string? ProductType)> GetRenewalAgentDataAsync(int renewalId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var renewal = await context.Renewals
                .Include(r => r.Client)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.RenewalId == renewalId);

            if (renewal == null)
            {
                return (null, null, null, null);
            }

            return (
                ClientId: renewal.Client?.ClientId,
                ProductId: renewal.Product?.ProductId,
                ClientName: renewal.Client?.Name,
                ProductType: renewal.Product?.LineName
            );
        }

        // AI RENEWAL DATA EXPORT ---------------------------------------------------//
        public async Task<List<string>> GetRenewalStringsForAIAsync(int clientId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            
            var today = DateTime.Today;
            var startDate = today.AddDays(-30);
            var endDate = today.AddDays(90);

            var renewals = await context.Renewals
                .Where(r => r.ClientId == clientId)
                .Where(r => r.RenewalDate >= startDate && r.RenewalDate <= endDate)
                .Include(r => r.Submissions)
                .Include(r => r.Product)
                .Include(r => r.Carrier)
                .Include(r => r.Wholesaler)
                .Include(r => r.Policy)
                .AsNoTracking()
                .ToListAsync();

            var renewalStrings = new List<string>();

            foreach (var renewal in renewals)
            {
                // Calculate the current status based on submissions
                var maxSubmissionStatus = renewal.Submissions.Any() 
                    ? renewal.Submissions.Max(s => s.StatusInt) 
                    : (int?)null;

                // Determine status string based on StatusInt
                string statusString = maxSubmissionStatus switch
                {
                    1 => "CREATED",
                    2 => "STARTED", 
                    3 => "SUBMITTED",
                    4 => "QUOTED",
                    5 => "PROPOSED",
                    6 => "BOUND",
                    7 => "ISSUED",
                    _ => "NO SUBMISSIONS"
                };

                // Format the renewal string
                var renewalString = $"Renewal Date: {renewal.RenewalDate:M/d/yyyy} | " +
                                   $"Coverage: {renewal.Product?.LineName ?? "Unknown"} | " +
                                   $"Carrier: {renewal.Carrier?.CarrierName ?? "Unknown"} | " +
                                   $"Wholesaler: {renewal.Wholesaler?.CarrierName ?? "Unknown"} | " +
                                   $"Policy#: {renewal.Policy?.PolicyNumber ?? "Unknown"} | " +
                                   $"Premium: ${renewal.Policy?.Premium:N0} | " +
                                   $"Status: {statusString}";

                renewalStrings.Add(renewalString);
            }

            return renewalStrings;
        }
    }
}
