using System.ComponentModel.DataAnnotations;
using Surefire.Domain.Clients.Models;

namespace Surefire.Domain.Contacts.Models
{
    public enum PhoneType
    {
        Office,
        Mobile,
        Home,
        Fax,
        Other
    }

    public class PhoneNumber
    {
        public int PhoneNumberId { get; set; }

        [Required]
        public string Number { get; set; }

        public string? Extension { get; set; }

        public PhoneType Type { get; set; }

        public string? TypeOther { get; set; }

        public bool IsPrimary { get; set; }

        public bool SMS { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime? DateModified { get; set; }

        // Navigation properties
        public int? ContactId { get; set; }
        public Contact? Contact { get; set; }

        //public int? ClientId { get; set; }
        //public Client? Client { get; set; }
    }
}