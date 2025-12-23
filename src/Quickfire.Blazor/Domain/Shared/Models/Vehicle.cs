using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Quickfire.Blazor.Domain.Policies.Models;

namespace Quickfire.Blazor.Domain.Shared.Models
{
    public class Vehicle
    {
        [Key]
        public int VehicleId { get; set; }
        
        public string? Year { get; set; }
        
        public string? Make { get; set; }
        
        public string? Model { get; set; }
        
        public string? VIN { get; set; }
        public string? VehicleNumberNote { get; set; }
        
        public string? LicensePlate { get; set; }
        
        public DateTime? RegistrationDate { get; set; }
        public string? GaragedAddress { get; set; }
        public string? GaragedCity { get; set; }
        public string? GaragedState { get; set; }
        public string? GaragedPostalCode { get; set; }
        
        public string? CountryOfRegistration { get; set; }

        // Navigation property
        [Required]
        public int PolicyId { get; set; }
        
        [ForeignKey("PolicyId")]
        public virtual Policy Policy { get; set; }
    }
}
