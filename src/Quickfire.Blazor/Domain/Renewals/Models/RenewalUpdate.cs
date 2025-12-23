using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.Clients.Models;
using Quickfire.Blazor.Domain.Policies.Models;
using Quickfire.Blazor.Domain.Shared.Models;

namespace Quickfire.Blazor.Domain.Renewals.Models
{
    public class RenewalUpdate
    {
        public int RenewalUpdateId { get; set; }

        // Link and lifecycle
        public string RenewalHashId { get; set; } = string.Empty; // hash for public link
        public bool PortalEnabled { get; set; } = false;          // on/off switch
        public DateTime? ExpiresUtc { get; set; }                 // auto-set to now + 10 days when enabling

        // JSON payloads
        public string? OriginalJsonData { get; set; }             // initial payload pushed (for audit)
        public string? JsonData { get; set; }                     // latest payload (pushed or submitted)

        // Audit
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; }
        public DateTime? LastPushedUtc { get; set; }
        public DateTime? LastSubmittedUtc { get; set; }

        public ApplicationUser? CreatedBy { get; set; }
        public ApplicationUser? ModifiedBy { get; set; }

        // Flags & notes
        public bool ShowContractorInfo { get; set; } = false;
        public bool EzRenewal { get; set; } = false;
        public bool IncludeGeneralLiability { get; set; } = false;
        public bool IncludeWorkComp { get; set; } = false;
        public bool IncludeCommercialAuto { get; set; } = false;
        public int RenewalStage { get; set; } = 0;                // 0=new, 1=sent, 2=clientStarted, 3=submitted, 4=applied
        public string? NotesForClient { get; set; }
        public string? NotesForStaff { get; set; }
        public string? NotesFromClient { get; set; }
        public string? ExternalStatus { get; set; }

        // Foreign keys
        public Client Client { get; set; } = default!;
        public int ClientId { get; set; }

        public Renewal Renewal { get; set; } = default!;
        public int RenewalId { get; set; }

        public Policy? Policy { get; set; }
        public int? PolicyId { get; set; }

        public Product Product { get; set; } = default!;
        public int ProductId { get; set; }
    }
}


