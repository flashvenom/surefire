using Quickfire.Blazor.Domain.Clients.Models;
using Quickfire.Blazor.Data;

namespace Quickfire.Blazor.Domain.Policies.Models
{
    public class BusinessOwnersPolicyCoverage
    {
        public int BusinessOwnersPolicyCoverageId { get; set; }

        // Property coverages
        public int? BuildingLimit { get; set; }
        public int? BuildingDeductible { get; set; }
        public int? BusinessPersonalPropertyLimit { get; set; }
        public int? BusinessPersonalPropertyDeductible { get; set; }

        // Liability coverages with deductibles
        public int? LiabilityPerOccurrenceLimit { get; set; }
        public int? LiabilityPerOccurrenceDeductible { get; set; }
        public int? LiabilityAggregateLimit { get; set; }
        public int? LiabilityAggregateDeductible { get; set; }
        public int? ProfessionalLiabilityLimit { get; set; }
        public int? ProfessionalLiabilityDeductible { get; set; }
        public int? EmploymentPracticesLimit { get; set; }
        public int? EmploymentPracticesDeductible { get; set; }
        public int? DirectorsAndOfficersLimit { get; set; }
        public int? DirectorsAndOfficersDeductible { get; set; }

        // Limit-only coverages
        public int? ProductsCompletedOperationsLimit { get; set; }
        public int? BodilyInjuryPropertyDamageLimit { get; set; }
        public int? PersonalAndAdvertisingInjuryLimit { get; set; }
        public int? TenantsLegalLiabilityLimit { get; set; }
        public int? HiredNonOwnedAutoLimit { get; set; }
        public int? BusinessIncomeLimit { get; set; }

        public string? BusinessIncomeType { get; set; }
        public string? LocationsJson { get; set; }

        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; } = DateTime.UtcNow;

        public Policy? Policy { get; set; }
        public int? PolicyId { get; set; }
        public Client? Client { get; set; }
        public int? ClientId { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public ApplicationUser? ModifiedBy { get; set; }
    }
}
