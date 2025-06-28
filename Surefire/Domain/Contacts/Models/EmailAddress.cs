using System.ComponentModel.DataAnnotations;
using Surefire.Domain.Clients.Models;

namespace Surefire.Domain.Contacts.Models
{
    public class EmailAddress
    {
        public int EmailAddressId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? Label { get; set; }

        public bool IsPrimary { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime? DateModified { get; set; }

        // Navigation properties
        public int? ContactId { get; set; }
        public Contact? Contact { get; set; }
    }
}