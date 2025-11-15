using System.Data;
using Surefire.Data;
using Surefire.Domain.Policies.Models;
using Microsoft.EntityFrameworkCore;
using Surefire.Domain.Carriers.Models;
using Surefire.Domain.Shared.Models;
using Surefire.Domain.Shared.Services;


namespace Surefire.Domain.Policies.Services
{
    public class PolicyService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly StateService _stateService;

        public PolicyService(ApplicationDbContext context, IDbContextFactory<ApplicationDbContext> dbContextFactory, StateService stateService)
        {
            _context = context;
            _dbContextFactory = dbContextFactory;
            _stateService = stateService;
        }

        // POLICY [GET] -----------------------------------------------------//
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
        public IQueryable<Policy> GetAllPolicies()
        {
            return _context.Policies
                .Include(p => p.Carrier)
                .Include(p => p.Wholesaler)
                .Include(p => p.Client)
                .Include(p => p.Product)
                .Select(p => new Policy
                {
                    PolicyId = p.PolicyId,
                    PolicyNumber = p.PolicyNumber,
                    EffectiveDate = p.EffectiveDate,
                    ExpirationDate = p.ExpirationDate,
                    Premium = p.Premium,
                    Product = p.Product ?? new Product { LineNickname = "N/A" },  // Handle null Product
                    Carrier = p.Carrier ?? new Carrier { CarrierName = "N/A" },    // Handle null Carrier
                    Wholesaler = p.Wholesaler ?? new Carrier { CarrierName = "N/A" }, // Handle null Wholesaler
                    Client = p.Client
                })
                .AsQueryable();
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
        public async Task<List<Policy>> GetCurrentPoliciesByCarrierIdAsync(int carrierId)
        {
            var today = DateTime.UtcNow.Date;

            var policies = await _context.Policies
                .Include(p => p.Carrier)
                .Include(p => p.Wholesaler)
                .Include(p => p.Product)
                .Include(p => p.Client)
                .Where(p => (p.CarrierId == carrierId || p.WholesalerId == carrierId) && p.EffectiveDate <= today && p.ExpirationDate >= today)
                .OrderBy(p => p.ExpirationDate)
                .ToListAsync();

            return policies;
        }

        // POLICY [CRUD] ----------------------------------------------------//
        public async Task<int> CreatePolicyAsync(Policy policy, int clientId)
        {
            var clientExists = await _context.Clients.AnyAsync(c => c.ClientId == clientId);
            if (!clientExists)
            {
                throw new ArgumentException("Invalid ClientId. The specified client does not exist.");
            }

            var currentUser = _stateService.CurrentUser;
            Policy newPolicy = new Policy
            {
                ClientId = clientId,
                CSR = currentUser,
                CreatedBy = currentUser,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                ProductId = policy.ProductId,
                PolicyNumber = policy.PolicyNumber,
                Premium = policy.Premium,
                Notes = policy.Notes,
                Status = policy.Status,
                EffectiveDate = policy.EffectiveDate,
                ExpirationDate = policy.ExpirationDate,
                CarrierId = policy.CarrierId,
                WholesalerId = policy.CarrierId
            };
            _context.Policies.Add(newPolicy);
            await _context.SaveChangesAsync();
            return newPolicy.PolicyId;
        }
        public async Task<int> CreatePolicyForClientAsync(PolicyCreate policy, int clientId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var clientExists = await context.Clients.AnyAsync(c => c.ClientId == clientId);
            if (!clientExists)
            {
                throw new ArgumentException("Invalid ClientId. The specified client does not exist.");
            }

            var currentUser = _stateService.CurrentUser;
            context.Attach(currentUser);
            Policy newPolicy = new Policy
            {
                ClientId = clientId,
                CSR = currentUser,
                CreatedBy = currentUser,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                ProductId = policy.ProductId,
                PolicyNumber = policy.PolicyNumber,
                Premium = policy.Premium ?? 0,
                EffectiveDate = policy.EffectiveDate,
                ExpirationDate = policy.ExpirationDate,
                CarrierId = policy.CarrierId,
                WholesalerId = policy.WholesalerId
            };
            context.Policies.Add(newPolicy);
            await context.SaveChangesAsync();
            return newPolicy.PolicyId;
        }
        public async Task UpdatePolicyContextModelAsync(Policy policy)
        {
            _context.Entry(policy).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        // RATING BASIS -----------------------------------------------------//
        public async Task DeleteRatingBasisAsync(int ratingBasisId)
        {
            var ratingBasis = await _context.RatingBases.FindAsync(ratingBasisId);
            if (ratingBasis == null) throw new KeyNotFoundException("Rating Basis not found");

            _context.RatingBases.Remove(ratingBasis);
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
    }
}
