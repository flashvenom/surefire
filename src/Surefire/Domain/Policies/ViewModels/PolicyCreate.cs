using Surefire.Domain.Shared.Models;
using Surefire.Domain.Carriers.Models;
using System.ComponentModel.DataAnnotations;

namespace Surefire.Domain.Policies.Models
{
    public class PolicyCreate
    {
        [Required(ErrorMessage = "Policy Number is required.")]
        public string PolicyNumber { get; set; }
        [Required(ErrorMessage = "Effective Date is required.")]
        public DateTime EffectiveDate { get; set; }
        [Required(ErrorMessage = "Expiration Date is required.")]
        public DateTime ExpirationDate { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }

        public Carrier? Carrier { get; set; }
        public int? CarrierId { get; set; }
        public Carrier? Wholesaler { get; set; }
        public int? WholesalerId { get; set; }
        public decimal? Premium { get; set; }
        public int? ClientId { get; set; }
    }
}
