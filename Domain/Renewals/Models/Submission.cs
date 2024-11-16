using Mantis.Domain.Carriers.Models;
using Mantis.Domain.Clients.Models;
using Mantis.Domain.Shared;
using Mantis.Domain.Shared.Models;
using Microsoft.CodeAnalysis.Elfie.Serialization;

namespace Mantis.Domain.Renewals.Models
{
    public class Submission
    {
        public int SubmissionId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string? Status { get; set; }
        public string? SubmissionStatus { get; set; }
        public int StatusInt { get; set; } = 1;
        public string? Notes { get; set; }

        public int? Premium { get; set; }
        public int? PrimaryCarrierContactId { get; set; }
        public int? PrimaryWholesalerContactId { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; } = DateTime.UtcNow;
        public DateTime? DateDeleted { get; set; } = DateTime.UtcNow;
        public Product Product { get; set; }
        public Carrier? Carrier { get; set; }
        public Carrier? Wholesaler { get; set; }
        public Renewal? Renewal { get; set; }
        public Lead? Lead { get; set; }
        public int? LeadId { get; set; }
        public List<SubmissionNote> SubmissionNotes { get; set; } = new List<SubmissionNote>();
    }

    public class SubmissionNote
    {
        public int SubmissionNoteId { get; set; }
        public DateTime DateCreated { get; set; }
        public string Note { get; set; }
        public Submission Submission { get; set; }
        public int SubmissionId { get; set; }
        public bool Deleted { get; set; }
    }

    public enum SubmissionStatus
    {
        Started,
        Submitted,
        Underwriting,
        Quoted,
        Proposed,
        Declined,
        Accepted
    }
}
