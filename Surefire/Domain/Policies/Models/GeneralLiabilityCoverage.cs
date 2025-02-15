﻿using Surefire.Domain.Clients.Models;
using Surefire.Data;
using Surefire.Domain.Attachments.Models;

namespace Surefire.Domain.Policies.Models
{
    public class GeneralLiabilityCoverage
    {
        public int GeneralLiabilityCoverageId { get; set; }

        //Limits
        public int? EachOccurrence { get; set; }
        public int? DamageToPremises { get; set; }
        public int? MedicalExpenses { get; set; }
        public int? PersonalInjury { get; set; }
        public int? GeneralAggregate { get; set; }
        public int? ProductsAggregate { get; set; }
        public string? AdditionalCoverageName { get; set; }
        public int? AdditionalCoverageLimit { get; set; }

        //Options
        public decimal? Premium { get; set; }
        public bool? ClaimsMade { get; set; }
        public bool? Occurence { get; set; }
        public int? AggregateAppliesPer { get; set; }

        //Attachments
        public bool? AdditionalInsured { get; set; }
        public string? AdditionalInsuredFormNumber { get; set; }
        public Attachment? AdditionalInsuredAttachment { get; set; }
        public bool? PrimaryWording { get; set; }
        public bool? WaiverOfSub { get; set; }
        public Attachment? WaiverOfSubAttachment { get; set; }
        public bool? AdditionalAttachments { get; set; }
        public Attachment? AdditionalAttachmentsAttachment { get; set; }

        //Record Info
        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public Policy? Policy { get; set; }
        public int? PolicyId { get; set; }
        public Client? Client { get; set; }
        public int? ClientId { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public ApplicationUser? ModifiedBy { get; set; }
    }

}
