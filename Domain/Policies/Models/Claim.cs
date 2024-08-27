using Mantis.Domain.Shared.Models;

namespace Mantis.Domain.Policies.Models
{
    public class Claim
    {
        public int ClaimId { get; set; }
        public string ClaimNumber { get; set; }
        public DateTime DateOfLoss { get; set; }
        public decimal AmountPaid { get; set; }

        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}
