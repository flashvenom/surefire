using Mantis.Data;

namespace Mantis.Domain.Policies.Models
{
    public class Loss
    {
        public int LossId { get; set; }
        public DateTime? DateOccurred { get; set; }
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public DateTime? DateClaimSubmitted { get; set; }
        public decimal? AmountPaid { get; set; }
        public decimal? AmountReserved { get; set; }
        public bool? Subgrogated { get; set; }
        public bool? Open { get; set; }

        //FKs
        public ApplicationUser? UserModified { get; set; }
        public Policy? Policy { get; set; }
        public int? PolicyId { get; set; }
    }

}
