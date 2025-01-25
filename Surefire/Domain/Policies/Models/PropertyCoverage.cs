using Surefire.Domain.Shared;
using Surefire.Domain.Carriers.Models;
using Surefire.Domain.Clients.Models;
using Surefire.Domain.Renewals.Models;
using System.Text.Json.Serialization;
using Surefire.Data;

namespace Surefire.Domain.Policies.Models
{
    public class PropertyCoverage
    {
        public int PropertyCoverageId { get; set; }

        //Limits
        public int? BusinessPersonalProperty { get; set; }
        public int? Equipment { get; set; }

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
