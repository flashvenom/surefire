using Folient.Domain.Shared;
using Folient.Domain.Clients.Models;
using System.Net;
using Folient.Domain.Carriers.Models;

namespace Folient.Domain.Shared
{
    public class Address
    {
        public int AddressId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        
    }
}
