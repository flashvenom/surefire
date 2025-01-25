using Surefire.Data;
using Surefire.Domain.Contacts.Models;
using Surefire.Domain.Shared.Models;
using System.ComponentModel.DataAnnotations;
using Surefire.Domain.Attachments.Models;

namespace Surefire.Domain.Carriers.Models
{
    public class Carrier
    {
        public int CarrierId { get; set; }
        [Required]
        public string LookupCode { get; set; }
        [Required]
        public string CarrierName { get; set; }
        public string? CarrierNickname { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public string? QuotingWebsite { get; set; }
        public string? ServicingWebsite { get; set; }
        public string? NewSubmissionEmail { get; set; }
        public string? ServicingEmail { get; set; }
        public string? LossRunsEmail { get; set; }
        public bool IssuingCarrier { get; set; }
        public bool Wholesaler { get; set; } = false;
        public bool QuickLink { get; set; } = false;
        public string? AppetiteJson { get; set; }
        public string? QuotelinesJson { get; set; }
        public string? Notes { get; set; }
        public string? LogoFilename { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; } = DateTime.UtcNow;
        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public ICollection<Credential> Credentials { get; set; } = new List<Credential>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public Address? Address { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
    }

    public class Credential
    {
        public int CredentialId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Website { get; set; }
        public string? Notes { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; } = DateTime.UtcNow;
        public ApplicationUser? CreatedBy { get; set; }
        public Carrier Carrier { get; set; }
        public int CarrierId { get; set; }
    }
}
