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

        public async Task<List<Policy>> GetCurrentPoliciesByClientIdAsync(int clientId)
        {
            var today = DateTime.UtcNow.Date;

            var policies = await _context.Policies
                .Include(p => p.Carrier)
                .Include(p => p.Wholesaler)
                .Include(p => p.Product)
                .Include(p => p.GeneralLiabilityCoverage)
                    .ThenInclude(glc => glc.AdditionalInsuredAttachment)
                .Include(p => p.GeneralLiabilityCoverage)
                    .ThenInclude(glc => glc.WaiverOfSubAttachment)
                .Include(p => p.UmbrellaCoverage)
                .Include(p => p.WorkCompCoverage)
                    .ThenInclude(glc => glc.WaiverOfSubAttachment)
                .Include(p => p.PropertyCoverage)
                .Include(p => p.AutoCoverage)
                .Where(p => p.ClientId == clientId && p.EffectiveDate <= today && p.ExpirationDate >= today)
                .ToListAsync();

            return policies;
        }
        //
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

        public async Task<Policy> GetPolicyByIdAsync(int policyId)
        {
            var policy = await _context.Policies
                .Include(p => p.GeneralLiabilityCoverage)
                    .ThenInclude(glc => glc.AdditionalInsuredAttachment)
                .Include(p => p.GeneralLiabilityCoverage)
                    .ThenInclude(glc => glc.WaiverOfSubAttachment)
                .Include(p => p.WorkCompCoverage)
                .Include(p => p.AutoCoverage)
                .Include(p => p.PropertyCoverage)
                .Include(p => p.UmbrellaCoverage)
                .Include(p => p.Product)
                .Include(p => p.Client)
                .Include(p => p.Carrier)
                .Include(p => p.Wholesaler)
                .Include(p => p.RatingBases)
                .FirstOrDefaultAsync(p => p.PolicyId == policyId);

            if (policy == null) return null;

            //Add WorkComp to WC
            if (policy.Product.ProductId == 2 && policy.WorkCompCoverage == null)
            {
                var workCompCoverage = new WorkCompCoverage
                {
                    PolicyId = policy.PolicyId,
                };
                _context.WorkCompCoverages.Add(workCompCoverage);
                policy.WorkCompCoverage = workCompCoverage; //
            }

            //Add Liability to GL
            if (policy.Product.ProductId == 3 && policy.GeneralLiabilityCoverage == null)
            {
                var generalLiabilityCoverage = new GeneralLiabilityCoverage
                {
                    PolicyId = policy.PolicyId,
                };
                _context.GeneralLiabilityCoverages.Add(generalLiabilityCoverage);
                policy.GeneralLiabilityCoverage = generalLiabilityCoverage;
            }

            //Add Liability Coverage to BOP
            if (policy.Product.ProductId == 6 && policy.GeneralLiabilityCoverage == null)
            {
                var generalLiabilityCoverage = new GeneralLiabilityCoverage
                {
                    PolicyId = policy.PolicyId,
                };
                _context.GeneralLiabilityCoverages.Add(generalLiabilityCoverage);
                policy.GeneralLiabilityCoverage = generalLiabilityCoverage;
            }

            //Add Property Coverage to BOP
            if (policy.Product.ProductId == 6 && policy.PropertyCoverage == null)
            {
                var propertyCoverage = new PropertyCoverage
                {
                    PolicyId = policy.PolicyId,
                };
                _context.PropertyCoverage.Add(propertyCoverage);
                policy.PropertyCoverage = propertyCoverage;
            }

            //Add Property Coverage to Property
            if (policy.Product.ProductId == 14 && policy.PropertyCoverage == null)
            {
                var propertyCoverage = new PropertyCoverage
                {
                    PolicyId = policy.PolicyId,
                };
                _context.PropertyCoverage.Add(propertyCoverage);
                policy.PropertyCoverage = propertyCoverage;
            }

            //Add Auto COverage to Auto
            if (policy.Product.ProductId == 4 && policy.AutoCoverage == null)
            {
                var autoCoverage = new AutoCoverage
                {
                    PolicyId = policy.PolicyId,
                };
                _context.AutoCoverages.Add(autoCoverage);
                policy.AutoCoverage = autoCoverage;
            }

            //Add Umbrella Coverage to Umbrella
            if (policy.Product.ProductId == 7 && policy.UmbrellaCoverage == null)
            {
                var umbrellaCoverage = new UmbrellaCoverage
                {
                    PolicyId = policy.PolicyId,
                };
                _context.UmbrellaCoverage.Add(umbrellaCoverage);
                policy.UmbrellaCoverage = umbrellaCoverage;
            }

            await _context.SaveChangesAsync();

            return policy;
        }

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

        public async Task UpdateGLCoverageAsync(Policy policy)
        {
            _context.Entry(policy).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

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

        public async Task<RatingBasis> AddBlankRatingBasisAsync(int policyId)
        {
            var newRatingBasis = new RatingBasis
            {
                PolicyId = policyId,
            };

            _context.RatingBases.Add(newRatingBasis);
            await _context.SaveChangesAsync();

            return newRatingBasis;
        }

        public async Task DeleteRatingBasisAsync(int ratingBasisId)
        {
            var ratingBasis = await _context.RatingBases.FindAsync(ratingBasisId);
            if (ratingBasis == null) throw new KeyNotFoundException("Rating Basis not found");

            _context.RatingBases.Remove(ratingBasis);
            await _context.SaveChangesAsync();
        }
    }
}
