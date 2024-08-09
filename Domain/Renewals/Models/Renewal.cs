using Mantis.Data;
using Mantis.Domain.Carriers.Models;
using Mantis.Domain.Policies.Models;
using Mantis.Domain.Clients.Models;
using Mantis.Domain.Shared;
using System.Collections.Generic;

namespace Mantis.Domain.Renewals.Models
{
    public class Renewal
    {
        public int RenewalId { get; set; }
        public DateTime RenewalDate { get; set; }
        public string? ExpiringPolicyNumber { get; set; }
        public decimal? ExpiringPremium { get; set; }
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
        public ICollection<TrackTask> TrackTasks { get; set; } = new List<TrackTask>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
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

    }
}
