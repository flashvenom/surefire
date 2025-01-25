using Surefire.Domain.Shared;
using Surefire.Domain.Carriers.Models;
using Surefire.Domain.Clients.Models;
using Surefire.Domain.Renewals.Models;
using System.Text.Json.Serialization;
using Surefire.Data;

namespace Surefire.Domain.Policies.Models
{
    public class UmbrellaCoverage
    {
        public int UmbrellaCoverageId { get; set; }

        //Type
        public bool? IsUmbrella { get; set; }
        public bool? IsExcess { get; set; }
        public bool? ClaimsMade { get; set; }
        public bool? Occurrence { get; set; }
        public bool? HasRetention { get; set; }
        public bool? HasDeductible { get; set; }


        //Limits
        public int? EachOccurrence { get; set; }
        public int? GeneralAggregate { get; set; }
        public int? DeductibleRetentionAmount { get; set; }

        //Record Info
        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public Policy? Policy{ get; set; }
        public int? PolicyId { get; set; }
        public Client? Client { get; set; }
        public int? ClientId { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public ApplicationUser? ModifiedBy { get; set; }
    }

}
