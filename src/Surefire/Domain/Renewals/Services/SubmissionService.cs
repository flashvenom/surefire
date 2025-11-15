using Microsoft.EntityFrameworkCore;
using Surefire.Data;
using Surefire.Domain.Carriers.Models;
using Surefire.Domain.Renewals.Models;

namespace Surefire.Domain.Shared.Services
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
                .Include(s => s.Wholesaler)
                    .ThenInclude(w => w.Contacts)
                .Include(s => s.Product)
                .Include(s => s.SubmissionNotes)
                .Include(s => s.Attachments)
                .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);

            // Order the SubmissionNotes by DateCreated descending
            submission.SubmissionNotes = submission.SubmissionNotes.OrderByDescending(sn => sn.DateCreated).ToList();

            return submission;
        }
        public async Task<Submission> CreateNewSubmissionAsync(int parentId, string type, int? carrierId = null, int? wholesalerId = null)
        {
            using var context = _dbContextFactory.CreateDbContext();
            Submission submission = new Submission
            {
                Premium = 0,
                StatusInt = 0,
                SubmissionDate = DateTime.Now
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
                    await context.SaveChangesAsync();
                }
                return carrier;
            }
            return null;
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
                    await context.SaveChangesAsync();
                }
                return wholesaler;
            }
            return null;
        }
        public async Task UpdateSubmissionPremiumAsync(int submissionId, int premium)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var submission = await context.Submissions.FindAsync(submissionId);
            if (submission != null)
            {
                submission.Premium = premium;
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteSubmissionAsync(int submissionId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var submission = await context.Submissions.FindAsync(submissionId);
            if (submission != null)
            {
                context.Submissions.Remove(submission);
                await context.SaveChangesAsync();
            }
        }
    }
}

