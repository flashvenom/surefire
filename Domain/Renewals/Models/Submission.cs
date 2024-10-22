using Mantis.Domain.Carriers.Models;
using Mantis.Domain.Shared;

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
        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; } = DateTime.UtcNow;

        public Product Product { get; set; }
        public Carrier? Carrier { get; set; }
        public Carrier? Wholesaler { get; set; }
        public Renewal? Renewal { get; set; }
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
