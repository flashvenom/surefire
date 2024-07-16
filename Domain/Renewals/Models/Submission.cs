using Folient.Domain.Carriers.Models;
using Folient.Domain.Shared;

namespace Folient.Domain.Policies.Models
{
    public class Submission
    {
        public int SubmissionId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Status { get; set; }

        public Product Product { get; set; }
        public Carrier Carrier { get; set; }
        public Carrier Wholesaler { get; set; }
    }
}
