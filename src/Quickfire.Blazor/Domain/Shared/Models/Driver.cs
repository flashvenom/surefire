using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Quickfire.Blazor.Domain.Policies.Models;

namespace Quickfire.Blazor.Domain.Shared.Models
{
    public class Driver
    {
        [Key]
        public int DriverId { get; set; }
        
        public string? FullName { get; set; }
        
        public DateTime? DateOfBirth { get; set; }
        
        public string? LicenseNumber { get; set; }
        public string? DriverNumberNote { get; set; }
        public string? LicenseState { get; set; }
        public string? Gender { get; set; }
        public string? Married { get; set; }
        
        public DateTime? LicenseExpiryDate { get; set; }
        public DateTime? DateOfHire { get; set; }
        
        public bool IsPrimaryDriver { get; set; }

        // Navigation property
        [Required]
        public int PolicyId { get; set; }
        
        [ForeignKey("PolicyId")]
        public virtual Policy Policy { get; set; }
    }
}
