using System.Data;
using Microsoft.AspNetCore.Identity;
using Mantis.Data;
using Mantis.Domain.Renewals.Models;
using Mantis.Domain.Renewals.ViewModels;
using Mantis.Domain.Policies.Models;
using Mantis.Domain.Clients.Models;
using Mantis.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Mantis.Domain.Renewals.ViewModels;


namespace Mantis.Data
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

        public async Task<List<Policy>> GetPoliciesAsync(string assignedUser = null, int? month = null, int? year = null)
        {
            var query = _context.Policies.AsQueryable();

            if (!string.IsNullOrEmpty(assignedUser))
            {
                query = query.Where(p => p.CSR == assignedUser);
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
    }
}
