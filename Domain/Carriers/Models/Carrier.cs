using Mantis.Data;
using Mantis.Domain.Contacts.Models;
using Mantis.Domain.Shared;
using System.ComponentModel.DataAnnotations;

using System.Collections.Generic;

namespace Mantis.Domain.Carriers.Models
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
        public bool IssuingCarrier { get; set; }
        public bool Wholesaler { get; set; } = false;
        public string? AppetiteJson { get; set; }
        public string? QuotelinesJson { get; set; }
        public string? Notes { get; set; }

        public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
        public Address? Address { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
    }
}
