using System.ComponentModel.DataAnnotations;

namespace Surefire.Domain.Clients.Models
{
    public class NewClientForm
    {
        [Required]
        public string Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        [Required]
        public string State { get; set; }
        public string? PostalCode { get; set; }
        [Required]
        public string ContactFirstName { get; set; }
        public string? ContactLastName { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? LookupCode { get; set; }
    }
}
