using Mantis.Data;
using Mantis.Domain.Contacts.Models;
using Mantis.Domain.Shared;
using System.Collections.Generic;

namespace Mantis.Domain.Carriers.Models
{
    public class Carrier
    {
        public int CarrierId { get; set; }
        public string LookupCode { get; set; }
        public string CarrierName { get; set; }
        public string? CarrierNickname { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public string? Phone { get; set; }
        public string? Website { get; set; }
        public string? QuotingWebsite { get; set; }
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
