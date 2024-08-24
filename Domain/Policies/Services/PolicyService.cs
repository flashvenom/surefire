using System.Data;
using Microsoft.AspNetCore.Identity;
using Mantis.Data;
using Mantis.Domain.Policies.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

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

        // 1. Get Policy by ID including related GeneralLiabilityCoverage
        public async Task<Policy> GetPolicyByIdAsync(int policyId)
        {
            return await _context.Policies
                .Include(p => p.GeneralLiabilityCoverage)
                .FirstOrDefaultAsync(p => p.PolicyId == policyId);
        }

        // 2. Update a specific field in the Policy model
        public async Task UpdatePolicyFieldAsync(int policyId, string fieldName, object value)
        {
            var policy = await _context.Policies.FindAsync(policyId);
            if (policy == null) throw new KeyNotFoundException("Policy not found");

            var property = policy.GetType().GetProperty(fieldName);
            if (property == null) throw new ArgumentException("Field not found on the Policy model");

            property.SetValue(policy, Convert.ChangeType(value, property.PropertyType));
            _context.Entry(policy).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        public async Task UpdatePolicyContextAsync(Policy policy, string fieldName, object value)
        {
            _context.Entry(policy).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        public async Task UpdatePolicyContextModelAsync(Policy policy)
        {
            _context.Entry(policy).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        // 3. Update a specific field in the GeneralLiabilityCoverage model
        public async Task UpdateGeneralLiabilityCoverageFieldAsync(int generalLiabilityCoverageId, string fieldName, object value)
        {
            var generalLiabilityCoverage = await _context.GeneralLiabilityCoverages.FindAsync(generalLiabilityCoverageId);
            if (generalLiabilityCoverage == null) throw new KeyNotFoundException("General Liability Coverage not found");

            var property = generalLiabilityCoverage.GetType().GetProperty(fieldName);
            if (property == null) throw new ArgumentException("Field not found on the GeneralLiabilityCoverage model");

            property.SetValue(generalLiabilityCoverage, Convert.ChangeType(value, property.PropertyType));
            _context.Entry(generalLiabilityCoverage).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

    }
}
