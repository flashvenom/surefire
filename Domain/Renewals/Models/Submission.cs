using Mantis.Domain.Carriers.Models;
using Mantis.Domain.Shared;

namespace Mantis.Domain.Renewals.Models
{
    public class Submission
    {
        public int SubmissionId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Status { get; set; }

        public Product Product { get; set; }
        public Carrier Carrier { get; set; }
        public Carrier Wholesaler { get; set; }
        public Renewal? Renewal { get; set; }
    }
}
