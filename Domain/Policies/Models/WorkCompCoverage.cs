using Mantis.Domain.Shared;
using Mantis.Domain.Carriers.Models;
using Mantis.Domain.Clients.Models;
using Mantis.Domain.Renewals.Models;
using System.Text.Json.Serialization;
using Mantis.Data;

namespace Mantis.Domain.Policies.Models
{
    public class WorkCompCoverage
    {
        public int WorkCompCoverageId { get; set; }

        //Limits
        public int? EachAccident { get; set; }
        public int? DiseaseEachEmployee { get; set; }
        public int? DiseasePolicyLimit { get; set; }

        //Options
        public bool? OwnersOfficersExcluded { get; set; }
        public bool? PerStatute { get; set; }
        public bool? PerOther { get; set; }

        //Attachments
        public bool? WaiverOfSub { get; set; }
        public Attachment? WaiverOfSubAttachment { get; set; }

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
