using Mantis.Domain.Shared;
using Mantis.Domain.Clients.Models;
using Mantis.Domain.Carriers.Models;

namespace Mantis.Domain.Contacts.Models
{
    public class Contact
    {
        public int ContactId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Title { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public string? Mobile { get; set; }
        public string? Notes { get; set; }

        public Address? Address { get; set; }
    }
}
