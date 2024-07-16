using Mantis.Domain.Carriers.Models;
using Mantis.Domain.Renewals.Models;
using Mantis.Domain.Shared;
using System.Collections.Generic;

namespace Mantis.Domain.Policies.Models
{
    public class Renewal
    {
        public int RenewalId { get; set; }
        public DateTime RenewalDate { get; set; }

        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
        public ICollection<TrackTask> TrackTasks { get; set; } = new List<TrackTask>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public Carrier Carrier { get; set; }
        public Carrier Wholesaler { get; set; }
    }
}
