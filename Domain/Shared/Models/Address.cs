using Mantis.Domain.Shared;
using Mantis.Domain.Clients.Models;
using System.Net;
using Mantis.Domain.Carriers.Models;

namespace Mantis.Domain.Shared
{
    public class Address
    {
        public int AddressId { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        
    }
}
