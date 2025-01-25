using Surefire.Data;
using Surefire.Domain.Clients.Models;
using Surefire.Domain.Carriers.Models;
using Surefire.Domain.Renewals.Models;

using Surefire.Domain.Policies.Models;

namespace Surefire.Domain.Attachments.Models
{
    public class Attachment
    {
        public int AttachmentId { get; set; } // Primary key
        public string? OriginalFileName { get; set; }
        public string HashedFileName { get; set; }
        public string LocalPath { get; set; }
        public string? FileFormat { get; set; }
        public double? FileSize { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateLastOpened { get; set; }
        public string Description { get; set; }
        public string? Comments { get; set; }
        public bool IsClientAccessible { get; set; }
        public int Status { get; set; }

        // Optional associations with other entities
        public int? ClientId { get; set; }
        public int? PolicyId { get; set; }
        public int? RenewalId { get; set; }
        public int? SubmissionId { get; set; }
        public int? CarrierId { get; set; }
        public int? AttachmentGroupId { get; set; }
        public int? FolderId { get; set; }
        public string? UploadedById { get; set; }

        // Policy-related classifications
        public bool IsPolicyCopy { get; set; }
        public bool IsEndorsement { get; set; }
        public bool IsBinder { get; set; }
        public bool IsQuote { get; set; }
        public bool IsProposal { get; set; }

        // Navigation properties
        public AttachmentGroup? AttachmentGroup { get; set; }
        public Folder? Folder { get; set; }
        public ApplicationUser? UploadedBy { get; set; }
        public Client? Client { get; set; }
        public Policy? Policy { get; set; }
        public Renewal? Renewal { get; set; }
        public Carrier? Carrier { get; set; }
        public Submission? Submission { get; set; }
    }

    public class AttachmentGroup
    {
        public int AttachmentGroupId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Attachment> Attachments { get; set; }
    }

    public class Folder
    {
        public int FolderId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Attachment> Attachments { get; set; }
    }
}
