using System.Data;
using System.Linq;
using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.Policies.Models;
using Microsoft.EntityFrameworkCore;
using Quickfire.Blazor.Domain.Carriers.Models;
using Quickfire.Blazor.Domain.Shared.Models;
using Quickfire.Blazor.Domain.Shared.Services;
using Quickfire.Blazor.Domain.Shared.Helpers;


namespace Quickfire.Blazor.Domain.Policies.Services
{
    public partial class PolicyService
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
        // Get policies for a client and line of business within the last N years
        public async Task<List<Policy>> GetPoliciesForClientAndLineAsync(int clientId, int productId, int years)
        {
            var cutoffDate = DateTime.UtcNow.AddYears(-years);
            return await _context.Policies
                .Include(p => p.Product)
                .Include(p => p.Carrier)
                .Where(p => p.ClientId == clientId
                            && p.Product != null
                            && p.Product.ProductId == productId
                            && p.EffectiveDate >= cutoffDate)
                .OrderByDescending(p => p.EffectiveDate)
                .ToListAsync();
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
                .Include(p => p.BusinessOwnersPolicyCoverage)
                .Include(p => p.UmbrellaCoverage)
                .Include(p => p.Product)
                .Include(p => p.Client)
                    .ThenInclude(c => c.Address)
                .Include(p => p.Client)
                    .ThenInclude(c => c.PrimaryContact)
                        .ThenInclude(pc => pc.EmailAddresses)
                .Include(p => p.Client)
                    .ThenInclude(c => c.PrimaryContact)
                        .ThenInclude(pc => pc.PhoneNumbers)
                .Include(p => p.Carrier)
                .Include(p => p.Wholesaler)
                .Include(p => p.Vehicles)
                .Include(p => p.Drivers)
                .Include(p => p.RatingBases)
                .FirstOrDefaultAsync(p => p.PolicyId == policyId);

            if (policy == null) return null;

            //Add WorkComp to WC
            if (ProductLineCodes.MatchesLineCode(policy.Product, ProductLineCodes.WorkersComp) && policy.WorkCompCoverage == null)
            {
                var workCompCoverage = new WorkCompCoverage
                {
                    PolicyId = policy.PolicyId,
                };
                _context.WorkCompCoverages.Add(workCompCoverage);
                policy.WorkCompCoverage = workCompCoverage; //
            }

            //Add Liability to GL
            if (ProductLineCodes.MatchesLineCode(policy.Product, ProductLineCodes.GeneralLiability) && policy.GeneralLiabilityCoverage == null)
            {
                var generalLiabilityCoverage = new GeneralLiabilityCoverage
                {
                    PolicyId = policy.PolicyId,
                };
                _context.GeneralLiabilityCoverages.Add(generalLiabilityCoverage);
                policy.GeneralLiabilityCoverage = generalLiabilityCoverage;
            }

            //Add Liability Coverage to BOP
            if (ProductLineCodes.MatchesLineCode(policy.Product, ProductLineCodes.BusinessOwners) && policy.GeneralLiabilityCoverage == null)
            {
                var generalLiabilityCoverage = new GeneralLiabilityCoverage
                {
                    PolicyId = policy.PolicyId,
                };
                _context.GeneralLiabilityCoverages.Add(generalLiabilityCoverage);
                policy.GeneralLiabilityCoverage = generalLiabilityCoverage;
            }

            //Add BOP Coverage to BOP
            if (ProductLineCodes.MatchesLineCode(policy.Product, ProductLineCodes.BusinessOwners) && policy.BusinessOwnersPolicyCoverage == null)
            {
                var businessOwnersCoverage = new BusinessOwnersPolicyCoverage
                {
                    PolicyId = policy.PolicyId,
                };
                _context.BusinessOwnersPolicyCoverages.Add(businessOwnersCoverage);
                policy.BusinessOwnersPolicyCoverage = businessOwnersCoverage;
            }

            //Add Property Coverage to BOP
            if (ProductLineCodes.MatchesLineCode(policy.Product, ProductLineCodes.BusinessOwners) && policy.PropertyCoverage == null)
            {
                var propertyCoverage = new PropertyCoverage
                {
                    PolicyId = policy.PolicyId,
                };
                _context.PropertyCoverage.Add(propertyCoverage);
                policy.PropertyCoverage = propertyCoverage;
            }

            //Add Property Coverage to Property
            if (ProductLineCodes.MatchesLineCode(policy.Product, ProductLineCodes.Property) && policy.PropertyCoverage == null)
            {
                var propertyCoverage = new PropertyCoverage
                {
                    PolicyId = policy.PolicyId,
                };
                _context.PropertyCoverage.Add(propertyCoverage);
                policy.PropertyCoverage = propertyCoverage;
            }

            //Add Auto COverage to Auto
            if (ProductLineCodes.MatchesLineCode(policy.Product, ProductLineCodes.CommercialAuto) && policy.AutoCoverage == null)
            {
                var autoCoverage = new AutoCoverage
                {
                    PolicyId = policy.PolicyId,
                };
                _context.AutoCoverages.Add(autoCoverage);
                policy.AutoCoverage = autoCoverage;
            }

            //Add Umbrella Coverage to Umbrella
            if (ProductLineCodes.MatchesLineCode(policy.Product, ProductLineCodes.Umbrella) && policy.UmbrellaCoverage == null)
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
            await using var context = await _dbContextFactory.CreateDbContextAsync();
            var today = DateTime.UtcNow.Date;

            var policies = await context.Policies
                .AsNoTracking()
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
                .Include(p => p.BusinessOwnersPolicyCoverage)
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
            using var context = _dbContextFactory.CreateDbContext();
            context.Entry(policy).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        // RATING BASIS -----------------------------------------------------//
        public async Task DeleteRatingBasisAsync(int ratingBasisId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var ratingBasis = await context.RatingBases.FindAsync(ratingBasisId);
            if (ratingBasis == null) throw new KeyNotFoundException("Rating Basis not found");

            context.RatingBases.Remove(ratingBasis);
            await context.SaveChangesAsync();
        }
        public async Task<RatingBasis> AddBlankRatingBasisAsync(int policyId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var newRatingBasis = new RatingBasis
            {
                PolicyId = policyId,
            };

            context.RatingBases.Add(newRatingBasis);
            await context.SaveChangesAsync();

            return newRatingBasis;
        }

        // WORK COMP RATING BASIS ----------------------------------------//
        public async Task<WorkCompRatingBasis> UpsertWorkCompRatingBasisAsync(WorkCompRatingBasis workCompRatingBasis)
        {
            using var context = _dbContextFactory.CreateDbContext();

            var existingRecords = await context.WorkCompRatingBases
                .AsNoTracking()
                .Where(rb => rb.PolicyId == workCompRatingBasis.PolicyId)
                .ToListAsync();

            var matchingRecord = existingRecords.FirstOrDefault(rb => WorkCompRatingBasisMatches(rb, workCompRatingBasis));
            if (matchingRecord != null)
            {
                workCompRatingBasis.WorkCompRatingBasisId = matchingRecord.WorkCompRatingBasisId;
                return matchingRecord;
            }

            workCompRatingBasis.DateCreated ??= DateTime.UtcNow;
            workCompRatingBasis.DateModified = DateTime.UtcNow;

            context.WorkCompRatingBases.Add(workCompRatingBasis);
            await context.SaveChangesAsync();

            return workCompRatingBasis;
        }

        // WORK COMP COVERAGE ----------------------------------------//
        public async Task<WorkCompCoverage> UpsertWorkCompCoverageAsync(WorkCompCoverage workCompCoverage)
        {
            using var context = _dbContextFactory.CreateDbContext();
            // Check if a coverage record already exists for this policy
            var existingCoverage = await context.WorkCompCoverages
                .FirstOrDefaultAsync(wcc => wcc.PolicyId == workCompCoverage.PolicyId);

            if (existingCoverage != null)
            {
                // Update existing record
                existingCoverage.EachAccident = workCompCoverage.EachAccident;
                existingCoverage.DiseaseEachEmployee = workCompCoverage.DiseaseEachEmployee;
                existingCoverage.DiseasePolicyLimit = workCompCoverage.DiseasePolicyLimit;
                existingCoverage.DateModified = DateTime.UtcNow;

                context.WorkCompCoverages.Update(existingCoverage);
                await context.SaveChangesAsync();
                
                return existingCoverage;
            }
            else
            {
                // Create new record
                workCompCoverage.DateCreated = DateTime.UtcNow;
                workCompCoverage.DateModified = DateTime.UtcNow;
                
                context.WorkCompCoverages.Add(workCompCoverage);
                await context.SaveChangesAsync();
                
                return workCompCoverage;
            }
        }

        // Get WorkCompCoverage by PolicyId
        public async Task<WorkCompCoverage?> GetWorkCompCoverageByPolicyIdAsync(int policyId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.WorkCompCoverages
                .Include(wcc => wcc.WaiverOfSubAttachment)
                .FirstOrDefaultAsync(wcc => wcc.PolicyId == policyId);
        }

        // Get WorkCompRatingBasis collection by PolicyId
        public async Task<List<WorkCompRatingBasis>> GetWorkCompRatingBasesByPolicyIdAsync(int policyId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.WorkCompRatingBases
                .Where(wcrb => wcrb.PolicyId == policyId)
                .ToListAsync();
        }

        // AUTO COVERAGE ----------------------------------------//
        public async Task<AutoCoverage> UpsertAutoCoverageAsync(AutoCoverage autoCoverage)
        {
            using var context = _dbContextFactory.CreateDbContext();
            // Check if a coverage record already exists for this policy
            var existingCoverage = await context.AutoCoverages
                .FirstOrDefaultAsync(ac => ac.PolicyId == autoCoverage.PolicyId);

            if (existingCoverage != null)
            {
                // Update existing record
                existingCoverage.CombinedLimit = autoCoverage.CombinedLimit;
                existingCoverage.BodilyInjuryPerPerson = autoCoverage.BodilyInjuryPerPerson;
                existingCoverage.BodilyInjuryPerAccident = autoCoverage.BodilyInjuryPerAccident;
                existingCoverage.PropertyDamage = autoCoverage.PropertyDamage;
                existingCoverage.ForAny = autoCoverage.ForAny;
                existingCoverage.ForOwned = autoCoverage.ForOwned;
                existingCoverage.ForHired = autoCoverage.ForHired;
                existingCoverage.ForScheduled = autoCoverage.ForScheduled;
                existingCoverage.ForNonOwned = autoCoverage.ForNonOwned;
                existingCoverage.DateModified = DateTime.UtcNow;

                context.AutoCoverages.Update(existingCoverage);
                await context.SaveChangesAsync();
                
                return existingCoverage;
            }
            else
            {
                // Create new record
                autoCoverage.DateCreated = DateTime.UtcNow;
                autoCoverage.DateModified = DateTime.UtcNow;
                
                context.AutoCoverages.Add(autoCoverage);
                await context.SaveChangesAsync();
                
                return autoCoverage;
            }
        }

        // Get AutoCoverage by PolicyId
        public async Task<AutoCoverage?> GetAutoCoverageByPolicyIdAsync(int policyId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.AutoCoverages
                .Include(ac => ac.AdditionalInsuredAttachment)
                .Include(ac => ac.WaiverOfSubAttachment)
                .Include(ac => ac.AdditionalAttachmentsAttachment)
                .FirstOrDefaultAsync(ac => ac.PolicyId == policyId);
        }

        // DRIVERS ----------------------------------------//
        public async Task<Driver> UpsertDriverAsync(Driver driver)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var existingDrivers = await context.Drivers
                .AsNoTracking()
                .Where(d => d.PolicyId == driver.PolicyId)
                .ToListAsync();

            var matchingDriver = existingDrivers.FirstOrDefault(d => DriverMatches(d, driver));
            if (matchingDriver != null)
            {
                driver.DriverId = matchingDriver.DriverId;
                return matchingDriver;
            }

            context.Drivers.Add(driver);
            await context.SaveChangesAsync();
            
            return driver;
        }

        // Get Drivers by PolicyId
        public async Task<List<Driver>> GetDriversByPolicyIdAsync(int policyId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.Drivers
                .Where(d => d.PolicyId == policyId)
                .ToListAsync();
        }

        // VEHICLES ----------------------------------------//
        public async Task<Vehicle> UpsertVehicleAsync(Vehicle vehicle)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var existingVehicles = await context.Vehicles
                .AsNoTracking()
                .Where(v => v.PolicyId == vehicle.PolicyId)
                .ToListAsync();

            var matchingVehicle = existingVehicles.FirstOrDefault(v => VehicleMatches(v, vehicle));
            if (matchingVehicle != null)
            {
                vehicle.VehicleId = matchingVehicle.VehicleId;
                return matchingVehicle;
            }

            context.Vehicles.Add(vehicle);
            await context.SaveChangesAsync();
            
            return vehicle;
        }

        // Get Vehicles by PolicyId
        public async Task<List<Vehicle>> GetVehiclesByPolicyIdAsync(int policyId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.Vehicles
                .Where(v => v.PolicyId == policyId)
                .ToListAsync();
        }

        // GENERAL LIABILITY COVERAGE ----------------------------------------//
        public async Task<GeneralLiabilityCoverage> UpsertGeneralLiabilityCoverageAsync(GeneralLiabilityCoverage glCoverage)
        {
            using var context = _dbContextFactory.CreateDbContext();
            // Check if a coverage record already exists for this policy
            var existingCoverage = await context.GeneralLiabilityCoverages
                .FirstOrDefaultAsync(glc => glc.PolicyId == glCoverage.PolicyId);

            if (existingCoverage != null)
            {
                // Update existing record
                existingCoverage.EachOccurrence = glCoverage.EachOccurrence;
                existingCoverage.DamageToPremises = glCoverage.DamageToPremises;
                existingCoverage.MedicalExpenses = glCoverage.MedicalExpenses;
                existingCoverage.PersonalInjury = glCoverage.PersonalInjury;
                existingCoverage.GeneralAggregate = glCoverage.GeneralAggregate;
                existingCoverage.ProductsAggregate = glCoverage.ProductsAggregate;
                existingCoverage.AdditionalCoverageName = glCoverage.AdditionalCoverageName;
                existingCoverage.AdditionalCoverageLimit = glCoverage.AdditionalCoverageLimit;
                existingCoverage.Premium = glCoverage.Premium;
                existingCoverage.ClaimsMade = glCoverage.ClaimsMade;
                existingCoverage.Occurence = glCoverage.Occurence;
                existingCoverage.AggregateAppliesPer = glCoverage.AggregateAppliesPer;
                existingCoverage.DateModified = DateTime.UtcNow;

                context.GeneralLiabilityCoverages.Update(existingCoverage);
                await context.SaveChangesAsync();
                
                return existingCoverage;
            }
            else
            {
                // Create new record
                glCoverage.DateCreated = DateTime.UtcNow;
                glCoverage.DateModified = DateTime.UtcNow;
                
                context.GeneralLiabilityCoverages.Add(glCoverage);
                await context.SaveChangesAsync();
                
                return glCoverage;
            }
        }

        // Get GeneralLiabilityCoverage by PolicyId
        public async Task<GeneralLiabilityCoverage?> GetGeneralLiabilityCoverageByPolicyIdAsync(int policyId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.GeneralLiabilityCoverages
                .Include(glc => glc.AdditionalInsuredAttachment)
                .Include(glc => glc.WaiverOfSubAttachment)
                .Include(glc => glc.AdditionalAttachmentsAttachment)
                .FirstOrDefaultAsync(glc => glc.PolicyId == policyId);
        }

        // BUSINESS OWNERS POLICY COVERAGE ----------------------------------------//
        public async Task<BusinessOwnersPolicyCoverage> UpsertBusinessOwnersPolicyCoverageAsync(BusinessOwnersPolicyCoverage bopCoverage)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var existingCoverage = await context.BusinessOwnersPolicyCoverages
                .FirstOrDefaultAsync(b => b.PolicyId == bopCoverage.PolicyId);

            if (existingCoverage != null)
            {
                existingCoverage.BuildingLimit = bopCoverage.BuildingLimit;
                existingCoverage.BuildingDeductible = bopCoverage.BuildingDeductible;
                existingCoverage.BusinessPersonalPropertyLimit = bopCoverage.BusinessPersonalPropertyLimit;
                existingCoverage.BusinessPersonalPropertyDeductible = bopCoverage.BusinessPersonalPropertyDeductible;
                existingCoverage.LiabilityPerOccurrenceLimit = bopCoverage.LiabilityPerOccurrenceLimit;
                existingCoverage.LiabilityPerOccurrenceDeductible = bopCoverage.LiabilityPerOccurrenceDeductible;
                existingCoverage.LiabilityAggregateLimit = bopCoverage.LiabilityAggregateLimit;
                existingCoverage.LiabilityAggregateDeductible = bopCoverage.LiabilityAggregateDeductible;
                existingCoverage.ProfessionalLiabilityLimit = bopCoverage.ProfessionalLiabilityLimit;
                existingCoverage.ProfessionalLiabilityDeductible = bopCoverage.ProfessionalLiabilityDeductible;
                existingCoverage.EmploymentPracticesLimit = bopCoverage.EmploymentPracticesLimit;
                existingCoverage.EmploymentPracticesDeductible = bopCoverage.EmploymentPracticesDeductible;
                existingCoverage.DirectorsAndOfficersLimit = bopCoverage.DirectorsAndOfficersLimit;
                existingCoverage.DirectorsAndOfficersDeductible = bopCoverage.DirectorsAndOfficersDeductible;
                existingCoverage.ProductsCompletedOperationsLimit = bopCoverage.ProductsCompletedOperationsLimit;
                existingCoverage.BodilyInjuryPropertyDamageLimit = bopCoverage.BodilyInjuryPropertyDamageLimit;
                existingCoverage.PersonalAndAdvertisingInjuryLimit = bopCoverage.PersonalAndAdvertisingInjuryLimit;
                existingCoverage.TenantsLegalLiabilityLimit = bopCoverage.TenantsLegalLiabilityLimit;
                existingCoverage.HiredNonOwnedAutoLimit = bopCoverage.HiredNonOwnedAutoLimit;
                existingCoverage.BusinessIncomeLimit = bopCoverage.BusinessIncomeLimit;
                existingCoverage.BusinessIncomeType = bopCoverage.BusinessIncomeType;
                existingCoverage.LocationsJson = bopCoverage.LocationsJson;
                existingCoverage.DateModified = DateTime.UtcNow;

                context.BusinessOwnersPolicyCoverages.Update(existingCoverage);
                await context.SaveChangesAsync();

                return existingCoverage;
            }

            bopCoverage.DateCreated = DateTime.UtcNow;
            bopCoverage.DateModified = DateTime.UtcNow;

            context.BusinessOwnersPolicyCoverages.Add(bopCoverage);
            await context.SaveChangesAsync();

            return bopCoverage;
        }

        public async Task<BusinessOwnersPolicyCoverage?> GetBusinessOwnersPolicyCoverageByPolicyIdAsync(int policyId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.BusinessOwnersPolicyCoverages
                .FirstOrDefaultAsync(b => b.PolicyId == policyId);
        }

        // RATING BASIS ----------------------------------------//
        public async Task<RatingBasis> UpsertRatingBasisAsync(RatingBasis ratingBasis)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var existingRecords = await context.RatingBases
                .AsNoTracking()
                .Where(rb => rb.PolicyId == ratingBasis.PolicyId)
                .ToListAsync();

            var matchingRecord = existingRecords.FirstOrDefault(rb => RatingBasisMatches(rb, ratingBasis));
            if (matchingRecord != null)
            {
                ratingBasis.RatingBasisId = matchingRecord.RatingBasisId;
                return matchingRecord;
            }

            ratingBasis.DateCreated ??= DateTime.UtcNow;
            ratingBasis.DateModified = DateTime.UtcNow;

            context.RatingBases.Add(ratingBasis);
            await context.SaveChangesAsync();

            return ratingBasis;
        }

        // Get RatingBasis collection by PolicyId
        public async Task<List<RatingBasis>> GetRatingBasesByPolicyIdAsync(int policyId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.RatingBases
                .Where(rb => rb.PolicyId == policyId)
                .ToListAsync();
        }

        public async Task<List<Policy>> GetPolicyLineHistoryAsync(Policy currentPolicy, int years = 5)
        {
            using var context = _dbContextFactory.CreateDbContext();
            if (currentPolicy == null || currentPolicy.Product == null)
            {
                return new List<Policy>();
            }

            var cutoffDate = DateTime.UtcNow.AddYears(-years);
            var clientId = currentPolicy.ClientId;
            var productName = currentPolicy.Product.LineName;

            var policies = await context.Policies
                .Include(p => p.Product)
                .Include(p => p.Carrier)
                .Where(p => p.ClientId == clientId && 
                            p.Product.LineName == productName && 
                            p.EffectiveDate >= cutoffDate &&
                            p.PolicyId != currentPolicy.PolicyId)
                .OrderByDescending(p => p.EffectiveDate)
                .ToListAsync();

            return policies;
        }

        public string FormatPolicyLineHistory(List<Policy> policies)
        {
            if (policies == null || !policies.Any())
            {
                return "No previous policy history found.";
            }

            var formattedHistory = new System.Text.StringBuilder();
            formattedHistory.AppendLine("<ul style=\"list-style-type: disc; margin-left: 20px;\">");

            foreach (var policy in policies)
            {
                formattedHistory.AppendLine($"<li><strong>{policy.PolicyNumber}</strong> - " +
                    $"{policy.Carrier?.CarrierName} - " +
                    $"Effective: {policy.EffectiveDate.ToShortDateString()} to {policy.ExpirationDate.ToShortDateString()} - " +
                    $"Premium: ${policy.Premium:N2}</li>");
            }

            formattedHistory.AppendLine("</ul>");
            return formattedHistory.ToString();
        }

        private static bool WorkCompRatingBasisMatches(WorkCompRatingBasis existing, WorkCompRatingBasis candidate)
        {
            return StringEquals(existing.ClassCode, candidate.ClassCode) &&
                   StringEquals(existing.ClassDescription, candidate.ClassDescription) &&
                   StringEquals(existing.LocationNumberNote, candidate.LocationNumberNote) &&
                   StringEquals(existing.LocationState, candidate.LocationState) &&
                   NullableDecimalEquals(existing.BaseRate, candidate.BaseRate) &&
                   NullableDecimalEquals(existing.NetRate, candidate.NetRate) &&
                   NullableDecimalEquals(existing.Premium, candidate.Premium) &&
                   NullableDecimalEquals(existing.Payroll, candidate.Payroll) &&
                   existing.FullTimeEmployees == candidate.FullTimeEmployees &&
                   existing.PartTimeEmployees == candidate.PartTimeEmployees;
        }

        private static bool RatingBasisMatches(RatingBasis existing, RatingBasis candidate)
        {
            return StringEquals(existing.ClassCode, candidate.ClassCode) &&
                   StringEquals(existing.ClassDescription, candidate.ClassDescription) &&
                   NullableDecimalEquals(existing.BaseRate, candidate.BaseRate) &&
                   NullableDecimalEquals(existing.NetRate, candidate.NetRate) &&
                   NullableDecimalEquals(existing.Premium, candidate.Premium) &&
                   NullableDecimalEquals(existing.Payroll, candidate.Payroll) &&
                   StringEquals(existing.Basis, candidate.Basis) &&
                   StringEquals(existing.Exposure, candidate.Exposure);
        }

        private static bool DriverMatches(Driver existing, Driver candidate)
        {
            return StringEquals(existing.DriverNumberNote, candidate.DriverNumberNote) &&
                   StringEquals(existing.FullName, candidate.FullName) &&
                   NullableDateEquals(existing.DateOfBirth, candidate.DateOfBirth) &&
                   StringEquals(existing.LicenseNumber, candidate.LicenseNumber) &&
                   StringEquals(existing.LicenseState, candidate.LicenseState) &&
                   StringEquals(existing.Gender, candidate.Gender) &&
                   StringEquals(existing.Married, candidate.Married) &&
                   NullableDateEquals(existing.LicenseExpiryDate, candidate.LicenseExpiryDate) &&
                   NullableDateEquals(existing.DateOfHire, candidate.DateOfHire) &&
                   existing.IsPrimaryDriver == candidate.IsPrimaryDriver;
        }

        private static bool VehicleMatches(Vehicle existing, Vehicle candidate)
        {
            return StringEquals(existing.VehicleNumberNote, candidate.VehicleNumberNote) &&
                   StringEquals(existing.Year, candidate.Year) &&
                   StringEquals(existing.Make, candidate.Make) &&
                   StringEquals(existing.Model, candidate.Model) &&
                   StringEquals(existing.VIN, candidate.VIN) &&
                   StringEquals(existing.LicensePlate, candidate.LicensePlate) &&
                   NullableDateEquals(existing.RegistrationDate, candidate.RegistrationDate) &&
                   StringEquals(existing.GaragedAddress, candidate.GaragedAddress) &&
                   StringEquals(existing.GaragedCity, candidate.GaragedCity) &&
                   StringEquals(existing.GaragedState, candidate.GaragedState) &&
                   StringEquals(existing.GaragedPostalCode, candidate.GaragedPostalCode) &&
                   StringEquals(existing.CountryOfRegistration, candidate.CountryOfRegistration);
        }

        private static bool StringEquals(string? left, string? right) =>
            string.Equals(NormalizeString(left), NormalizeString(right), StringComparison.OrdinalIgnoreCase);

        private static string NormalizeString(string? value) =>
            string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

        private static bool NullableDecimalEquals(decimal? left, decimal? right) =>
            left.HasValue == right.HasValue && (!left.HasValue || left.Value == right.Value);

        private static bool NullableDateEquals(DateTime? left, DateTime? right)
        {
            if (!left.HasValue && !right.HasValue) return true;
            if (!left.HasValue || !right.HasValue) return false;
            return left.Value.Date == right.Value.Date;
        }
    }
}
