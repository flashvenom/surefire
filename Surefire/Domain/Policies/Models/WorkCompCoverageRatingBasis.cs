using Surefire.Data;
using Surefire.Domain.Shared.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Surefire.Domain.Policies.Models
{
    [Table("WorkCompRatingBases")]
    public class WorkCompRatingBasis
    {
        //ID
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WorkCompRatingBasisId { get; set; }

        //Data
        public string? ClassCode { get; set; }

        public string? ClassDescription { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? BaseRate { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? NetRate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Premium { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Payroll { get; set; }

        public int? FullTimeEmployees { get; set; }

        public int? PartTimeEmployees { get; set; }
        public string? LocationNumberNote { get; set; }
        public string? LocationState { get; set; }

        //Record Info
        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime? DateModified { get; set; } = DateTime.UtcNow;

        //Foreign Keys and Navigation Properties
        [ForeignKey("UserModified")]
        public string? UserModifiedId { get; set; }
        public ApplicationUser? UserModified { get; set; }

        [Required]
        [ForeignKey("Policy")]
        public int PolicyId { get; set; }
        public Policy? Policy { get; set; }

        [ForeignKey("Location")]
        public int? LocationId { get; set; }
        public Location? Location { get; set; }
    }
}