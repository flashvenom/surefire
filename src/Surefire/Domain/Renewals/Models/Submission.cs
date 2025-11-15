using Surefire.Domain.Carriers.Models;
using Surefire.Domain.Clients.Models;
using Surefire.Domain.Shared.Models;
using Surefire.Domain.Attachments.Models;

namespace Surefire.Domain.Renewals.Models
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

        public RejectedStatus? RejectedStatus { get; set; }

        public List<SubmissionTask> SubmissionTasks { get; set; } = new List<SubmissionTask>();
        //Navigation
        public Product Product { get; set; }
        public Carrier? Carrier { get; set; }
        public Carrier? Wholesaler { get; set; }
        public Renewal? Renewal { get; set; }
        public Lead? Lead { get; set; }
        public int? LeadId { get; set; }
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }

    public enum RejectedStatus
    {
        Declined,       // Declined by the carrier
        Rejected,       // Rejected by the client
        NonRenewed      // A huge problem!
    }

    public class SubmissionTask
    {
        public int SubmissionTaskId { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; }

        public int SubmissionId { get; set; }
        public Submission Submission { get; set; }
    }

    public enum SubmissionStatus
    {
        Created,
        Started,
        Submitted,
        Quoted,
        Proposed,
        Bound,
        Issued
    }
}
