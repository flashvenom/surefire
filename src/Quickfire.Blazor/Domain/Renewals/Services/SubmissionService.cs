using Microsoft.EntityFrameworkCore;
using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.Carriers.Models;
using Quickfire.Blazor.Domain.Renewals.Models;
using Quickfire.Blazor.Domain.Attachments.Models;

namespace Quickfire.Blazor.Domain.Shared.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public SubmissionService(StateService stateService, IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }
        public async Task<Submission> GetSubmissionByIdAsync(int submissionId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var submission = await context.Submissions
                .Include(s => s.Carrier)
                    .ThenInclude(c => c.Contacts)
                        .ThenInclude(d => d.EmailAddresses)
                .Include(s => s.Carrier)
                    .ThenInclude(c => c.Contacts)
                        .ThenInclude(d => d.PhoneNumbers)
                .Include(s => s.Wholesaler)
                    .ThenInclude(w => w.Contacts)
                        .ThenInclude(d => d.EmailAddresses)
                .Include(s => s.Wholesaler)
                    .ThenInclude(c => c.Contacts)
                        .ThenInclude(d => d.PhoneNumbers)
                .Include(s => s.Product)
                .Include(s => s.SubmissionTasks)
                .Include(s => s.Attachments)
                .Include(s => s.Renewal)
                .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);

            return submission;
        }
        public async Task<Submission> CreateNewSubmissionAsync(int parentId, string type, int? carrierId = null, int? wholesalerId = null)
        {
            using var context = _dbContextFactory.CreateDbContext();
            Submission submission = new Submission
            {
                Premium = 0,
                StatusInt = 0,
                SubmissionDate = DateTime.Now,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now
            };

            if (carrierId.HasValue)
                submission.Carrier = await context.Carriers.FindAsync(carrierId);

            if (wholesalerId.HasValue)
                submission.Wholesaler = await context.Carriers.FindAsync(wholesalerId);

            if (type == "renewal")
            {
                var renewal = await context.Renewals.FindAsync(parentId);
                if (renewal != null)
                {
                    submission.Product = renewal.Product;
                    submission.Renewal = renewal;
                }
            }
            else if (type == "lead")
            {
                var lead = await context.Leads.FindAsync(parentId);
                if (lead != null)
                {
                    submission.Product = lead.Product;
                    submission.Lead = lead;
                    submission.LeadId = lead.LeadId;
                }
            }

            if (submission.Product == null)
            {
                submission.Product = await context.Products.FirstOrDefaultAsync();
                if (submission.Product == null)
                {
                    throw new Exception("No valid product found for submission.");
                }

            }

            context.Submissions.Add(submission);
            await context.SaveChangesAsync();
            return submission;
        }
        public async Task UpdateSubmissionAsync(Submission submission)
        {
            using var context = _dbContextFactory.CreateDbContext();
            submission.DateModified = DateTime.Now;
            context.Submissions.Attach(submission);
            context.Entry(submission).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
        public async Task UpdateSubmissionPrimaryContactAsync(int submissionId, int primaryContactId)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var submission = await context.Submissions.FindAsync(submissionId);
            if (submission != null)
            {
                submission.PrimaryWholesalerContactId = primaryContactId;
                submission.DateModified = DateTime.Now;
                await context.SaveChangesAsync();
            }

        }
        public async Task<Carrier> UpdateSubmissionCarrierAsync(int submissionId, int carrierId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var submission = await context.Submissions.FindAsync(submissionId);
            if (submission != null)
            {
                var carrier = await context.Carriers
                    .Include(c => c.Contacts) // Assuming Contacts is a navigation property on Carrier
                    .FirstOrDefaultAsync(c => c.CarrierId == carrierId);

                if (carrier != null)
                {
                    submission.Carrier = carrier;
                    submission.DateModified = DateTime.Now;
                    await context.SaveChangesAsync();
                }
                return carrier;
            }
            return null;
        }
        
        public async Task RemoveSubmissionCarrierAsync(int submissionId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var submission = await context.Submissions.FindAsync(submissionId);
            if (submission != null)
            {
                submission.Carrier = null;
                submission.PrimaryCarrierContactId = null;
                submission.DateModified = DateTime.Now;
                await context.SaveChangesAsync();
            }
        }
        
        public async Task<Carrier> UpdateSubmissionWholesalerAsync(int submissionId, int wholesalerId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var submission = await context.Submissions.FindAsync(submissionId);
            if (submission != null)
            {
                var wholesaler = await context.Carriers
                    .Include(c => c.Contacts) // Assuming Contacts is a navigation property on Carrier
                    .FirstOrDefaultAsync(c => c.CarrierId == wholesalerId);

                if (wholesaler != null)
                {
                    submission.Wholesaler = wholesaler;
                    submission.DateModified = DateTime.Now;
                    await context.SaveChangesAsync();
                }
                return wholesaler;
            }
            return null;
        }
        
        public async Task RemoveSubmissionWholesalerAsync(int submissionId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var submission = await context.Submissions.FindAsync(submissionId);
            if (submission != null)
            {
                submission.Wholesaler = null;
                submission.PrimaryWholesalerContactId = null;
                submission.DateModified = DateTime.Now;
                await context.SaveChangesAsync();
            }
        }
        
        public async Task UpdateSubmissionPremiumAsync(int submissionId, int premium)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var submission = await context.Submissions.FindAsync(submissionId);
            if (submission != null)
            {
                submission.Premium = premium;
                submission.DateModified = DateTime.Now;
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteSubmissionAsync(int submissionId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var submission = await context.Submissions
                
                .Include(s => s.SubmissionTasks)
                .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);
                
            if (submission != null)
            {
                // Remove all related notes and tasks first
                var submissionNotes = await context.RenewalNotes.Where(n => n.SubmissionId == submissionId).ToListAsync();
                context.RemoveRange(submissionNotes);
                context.RemoveRange(submission.SubmissionTasks);
                
                // Then remove the submission
                context.Submissions.Remove(submission);
                await context.SaveChangesAsync();
            }
        }
        
        // Task Management Methods
        public async Task<List<SubmissionTask>> GetSubmissionTasksAsync(int submissionId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.Set<SubmissionTask>()
                .Where(t => t.SubmissionId == submissionId)
                .OrderByDescending(t => t.DueDate)
                .ToListAsync();
        }
        
        public async Task<SubmissionTask> CreateSubmissionTaskAsync(int submissionId, string taskName, string description, DateTime? dueDate)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var submission = await context.Submissions.FindAsync(submissionId);
            
            if (submission == null)
                throw new Exception($"Submission with ID {submissionId} not found.");
                
            var task = new SubmissionTask
            {
                SubmissionId = submissionId,
                TaskName = taskName,
                Description = description ?? string.Empty,
                DueDate = dueDate,
                IsCompleted = false,
                DateCreated = DateTime.Now
            };
            
            context.Add(task);
            
            // Update the submission's modified date
            submission.DateModified = DateTime.Now;
            context.Update(submission);
            
            await context.SaveChangesAsync();
            return task;
        }
        
        public async Task<SubmissionTask> UpdateSubmissionTaskAsync(int taskId, string taskName, string description, DateTime? dueDate)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.Set<SubmissionTask>().FindAsync(taskId);
            
            if (task == null)
                throw new Exception($"Task with ID {taskId} not found.");
                
            task.TaskName = taskName;
            task.Description = description ?? string.Empty;
            task.DueDate = dueDate;
            task.DateModified = DateTime.Now;
            
            context.Update(task);
            
            // Update the submission's modified date
            var submission = await context.Submissions.FindAsync(task.SubmissionId);
            if (submission != null)
            {
                submission.DateModified = DateTime.Now;
                context.Update(submission);
            }
            
            await context.SaveChangesAsync();
            return task;
        }
        
        public async Task ToggleSubmissionTaskCompletionAsync(int taskId, bool isCompleted)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.Set<SubmissionTask>().FindAsync(taskId);
            
            if (task == null)
                throw new Exception($"Task with ID {taskId} not found.");
                
            task.IsCompleted = isCompleted;
            task.CompletedDate = isCompleted ? DateTime.Now : null;
            task.DateModified = DateTime.Now;
            
            context.Update(task);
            
            // Update the submission's modified date
            var submission = await context.Submissions.FindAsync(task.SubmissionId);
            if (submission != null)
            {
                submission.DateModified = DateTime.Now;
                context.Update(submission);
            }
            
            await context.SaveChangesAsync();
        }
        
        public async Task DeleteSubmissionTaskAsync(int taskId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var task = await context.Set<SubmissionTask>().FindAsync(taskId);
            
            if (task == null)
                throw new Exception($"Task with ID {taskId} not found.");
            
            int submissionId = task.SubmissionId;
            context.Remove(task);
            
            // Update the submission's modified date
            var submission = await context.Submissions.FindAsync(submissionId);
            if (submission != null)
            {
                submission.DateModified = DateTime.Now;
                context.Update(submission);
            }
            
            await context.SaveChangesAsync();
        }
        
        // Attachment Management
        public async Task<bool> DeleteAttachmentAsync(int attachmentId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var attachment = await context.Set<Attachment>().FindAsync(attachmentId);
            
            if (attachment == null)
                return false;
                
            try
            {
                // Find the related submission to update its modified date
                var submission = await context.Submissions
                    .FirstOrDefaultAsync(s => s.Attachments.Any(a => a.AttachmentId == attachmentId));
                    
                context.Remove(attachment);
                
                if (submission != null)
                {
                    submission.DateModified = DateTime.Now;
                    context.Update(submission);
                }
                
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

