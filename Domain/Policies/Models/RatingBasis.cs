using Mantis.Data;
using Mantis.Domain.Shared;

namespace Mantis.Domain.Policies.Models
{
    public class RatingBasis
    {
        //ID
        public int RatingBasisId { get; set; }
        
        //Data
        public string? ClassCode { get; set; }
        public string? ClassDescription { get; set; }
        public decimal? BaseRate { get; set; }
        public decimal? NetRate { get; set; }
        public decimal? Premium { get; set; }
        public decimal? Payroll { get; set; }
        public string? Basis { get; set; }
        public string? Exposure { get; set; }

        //Record Info
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        //Dependencies
        public ApplicationUser? UserModified { get; set; }
        public Policy? Policy { get; set; }
        public int? PolicyId { get; set; }
        public Product? Product { get; set; }
        public Location? Location { get; set; }

    }

}
