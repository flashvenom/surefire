using Mantis.Data;
using Mantis.Domain.Policies.Models;
using Mantis.Domain.Contacts.Models;
using Mantis.Domain.Forms.Models;
using Mantis.Domain.Shared;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Mantis.Domain.Clients.Models
{
    public class Client
    {
        public int ClientId { get; set; }
        public string? eClientId { get; set; }
        [Required]
        public string LookupCode { get; set; }
        [Required]
        public string Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? Comments { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        public ICollection<Policy> Policies { get; set; } = new List<Policy>();
        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public ICollection<Location> Locations { get; set; } = new List<Location>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

        public Address Address { get; set; }
        public Contact PrimaryContact { get; set; }
        public ApplicationUser? Producer { get; set; }
        public ApplicationUser? CSR { get; set; }
        public ApplicationUser CreatedBy { get; set; }
    }
}
