using Microsoft.EntityFrameworkCore;
using Quickfire.Blazor.Domain.Forms.Models;

namespace Quickfire.Blazor.Domain.Forms.Services
{
    public partial class FormService
    {
        public async Task<List<CertificateRequest>> GetInternalCertificateRequestsAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.CertificateRequests
                .Include(cr => cr.Client)
                .Include(cr => cr.CreatedBy)
                .Include(cr => cr.AssignedTo)
                .Include(cr => cr.Certificate)
                .OrderByDescending(cr => cr.RequestDate)
                .Take(10)
                .ToListAsync();
        }

        public async Task<CertificateRequest> CreateCertificateRequestAsync(CertificateRequest request)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var currentUser = _stateService.CurrentUser;
            
            request.CreatedBy = currentUser;
            request.RequestDate = DateTime.UtcNow;
            
            context.CertificateRequests.Add(request);
            await context.SaveChangesAsync();
            
            return request;
        }

        public async Task UpdateCertificateRequestAsync(CertificateRequest request)
        {
            using var context = _dbContextFactory.CreateDbContext();
            request.LastOpened = DateTime.UtcNow;
            context.CertificateRequests.Update(request);
            await context.SaveChangesAsync();
        }
    }
} 