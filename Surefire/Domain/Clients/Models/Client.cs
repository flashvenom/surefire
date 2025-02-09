using Surefire.Data;
using Surefire.Domain.Policies.Models;
using Surefire.Domain.Contacts.Models;
using Surefire.Domain.Forms.Models;
using Surefire.Domain.Shared.Models;
using System.ComponentModel.DataAnnotations;
using Surefire.Domain.Attachments.Models;

namespace Surefire.Domain.Clients.Models
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
        public string? LogoFilename { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public DateTime DateOpened { get; set; } = DateTime.UtcNow;

        public ICollection<Policy> Policies { get; set; } = new List<Policy>();
        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public ICollection<Location> Locations { get; set; } = new List<Location>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
        public ICollection<FormDoc> FormDocs { get; set; } = new List<FormDoc>();
        public ICollection<ClientNote> ClientNotes { get; set; } = new List<ClientNote>();

        public Address Address { get; set; }
        public int PrimaryContactId { get; set; }
        public Contact PrimaryContact { get; set; }
        public ApplicationUser? Producer { get; set; }
        public ApplicationUser? CSR { get; set; }
        public ApplicationUser CreatedBy { get; set; }
    }

    public class ClientListItem
    {
        public int ClientId { get; set; }
        public string Name { get; set; }
        public DateTime DateOpened { get; set; }
    }
}
