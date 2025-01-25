using Surefire.Data;
using Surefire.Domain.Carriers.Models;
using Surefire.Domain.Policies.Models;
using Surefire.Domain.Clients.Models;
using Surefire.Domain.Shared.Models;
using Surefire.Domain.Attachments.Models;
using Surefire.Domain.Accounting.Models;

namespace Surefire.Domain.Renewals.Models
{
    public class Renewal
    {
        public int RenewalId { get; set; }
        public DateTime RenewalDate { get; set; }
        public string? Notes { get; set; }
        public string? ExpiringPolicyNumber { get; set; }
        public decimal? ExpiringPremium { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; } = DateTime.UtcNow;

        public Carrier? Carrier { get; set; }
        public int? CarrierId { get; set; }
        public Carrier? Wholesaler { get; set; }
        public int? WholesalerId { get; set; }
        public Policy? Policy { get; set; }
        public int? PolicyId { get; set; }
        public Client Client { get; set; }
        public int ClientId { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public ApplicationUser AssignedTo { get; set; }
        public string AssignedToId { get; set; }

        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
        public ICollection<TrackTask> TrackTasks { get; set; } = new List<TrackTask>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public ICollection<Settlement> Settlements { get; set; } = new List<Settlement>();
    }
    public class RenewalListItemViewModel
    {
        public int RenewalId { get; set; }
        public DateTime RenewalDate { get; set; }
        public string? ProductLineCode { get; set; }
        public string? ClientName { get; set; }
        public string? CarrierName { get; set; }
        public string? WholesalerNickname { get; set; }
        public string? PolicyNumber { get; set; }
        public decimal? Premium { get; set; }
        public int? Submits { get; set; }
        public int ClientId { get; set; }
        public int? PolicyId { get; set; }
        public ICollection<TrackTask>? TrackTasks { get; set; }
        public string? AssignedToFirstName { get; set; }
        public string? AssignedToLastName { get; set; }
        public string? AssignedToPictureUrl { get; set; }
        public string? AssignedToId { get; set; }
    }
}
