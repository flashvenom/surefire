using Mantis.Domain.Clients.Models;
using Mantis.Domain.Shared.Models;
using Mantis.Data;

namespace Mantis.Domain.Policies.Models
{
    public class AutoCoverage
    {
        public int AutoCoverageId { get; set; }

        //Limits
        public int? CombinedLimit { get; set; }
        public int? BodilyInjuryPerPerson { get; set; }
        public int? BodilyInjuryPerAccident { get; set; }
        public int? PropertyDamage { get; set; }
        public string? AdditionalCoverageName { get; set; }
        public int? AdditionalCoverageLimit { get; set; }

        //Options
        public bool? ForAny { get; set; }
        public bool? ForOwned { get; set; }
        public bool? ForHired { get; set; }
        public bool? ForScheduled { get; set; } 
        public bool? ForNonOwned { get; set; }

        //Attachments
        public bool? AdditionalInsured { get; set; }
        public Attachment? AdditionalInsuredAttachment { get; set; }
        public bool? WaiverOfSub { get; set; }
        public Attachment? WaiverOfSubAttachment { get; set; }
        public bool? AdditionalAttachments { get; set; }
        public Attachment? AdditionalAttachmentsAttachment { get; set; }

        //Record Info
        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public Client? Client { get; set; }
        public int? ClientId { get; set; }
        public Policy? Policy { get; set; }
        public int? PolicyId { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public ApplicationUser? ModifiedBy { get; set; }
    }

}
