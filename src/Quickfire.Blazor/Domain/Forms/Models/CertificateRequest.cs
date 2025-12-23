using System;
using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.Clients.Models;

namespace Quickfire.Blazor.Domain.Forms.Models
{
    public class CertificateRequest
    {
        // Primary Key
        public int CertificateRequestId { get; set; }

        // Client Information
        public string ClientName { get; set; } = string.Empty;
        public string ClientCompany { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;

        // Certificate Holder Information
        public string HolderName { get; set; } = string.Empty;
        public string? HolderAttention { get; set; }
        public string HolderAddress { get; set; } = string.Empty;
        public string HolderCity { get; set; } = string.Empty;
        public string HolderState { get; set; } = string.Empty;
        public string HolderZip { get; set; } = string.Empty;
        public string? HolderEmail { get; set; }

        // Certificate Options
        public bool EmailToHolder { get; set; } = true;
        public bool EmailCopyToClient { get; set; } = true;
        public bool PrimaryNonContributary { get; set; }
        public bool WaiverSubrogationWC { get; set; }
        public bool WaiverSubrogationGL { get; set; }

        // Project Information
        public bool HasProject { get; set; }
        public int ProjectId { get; set; }
        public string? ProjectNumber { get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectAddress { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }

        // Status and Tracking
        public string Status { get; set; } = "Pending";
        public string? Notes { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastOpened { get; set; }

        // Internal System Relations
        public int? ClientId { get; set; }
        public Client? Client { get; set; }
        public string? AssignedToId { get; set; }
        public ApplicationUser? AssignedTo { get; set; }
        public string? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public int? CertificateId { get; set; }
        public Certificate? Certificate { get; set; }

        // External System Tracking
        public string? ExternalRequestId { get; set; }
        public DateTime? ImportedDate { get; set; }
        public bool IsImported { get; set; }

        // Additional properties as needed
        public string AdditionalInsured { get; set; }
        public string CoverageTypes { get; set; }
    }
} 