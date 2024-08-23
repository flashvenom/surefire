using System.Data;
using Microsoft.AspNetCore.Identity;
using Mantis.Data;
using Mantis.Domain.Policies.Models;
using Microsoft.EntityFrameworkCore;

namespace Mantis.Domain.Policies.Services
{
    public class PolicyService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PolicyService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Policy>> GetAllPoliciesAsync()
        {
            var policies = await _context.Policies.ToListAsync();
            return policies;
        }

        public async Task<List<Policy>> GetPoliciesAsync(string assignedUserId = null, int? month = null, int? year = null)
        {
            var query = _context.Policies.AsQueryable();
            var user = await _userManager.FindByIdAsync(assignedUserId);

            if (!string.IsNullOrEmpty(assignedUserId))
            {
                query = query.Where(p => p.CSR == user);
            }

            if (month.HasValue)
            {
                query = query.Where(p => p.ExpirationDate.Month == month.Value);
            }

            if (year.HasValue)
            {
                query = query.Where(p => p.ExpirationDate.Year == year.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<List<Policy>> GetUpcomingRenewalsAsync()
        {
            var currentDate = DateTime.UtcNow;
            var upcomingDate = currentDate.AddDays(14);

            var upcomingRenewals = await _context.Policies
                .Include(p => p.Product)
                .Include(p => p.Client)
                .Where(p => p.ExpirationDate >= currentDate && p.ExpirationDate <= upcomingDate)
                .ToListAsync();

            return upcomingRenewals;
        }
    }
}
