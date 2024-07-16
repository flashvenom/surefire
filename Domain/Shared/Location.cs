using Folient.Domain.Shared;

namespace Folient.Domain.Clients.Models
{
    public class Location
    {
        public int LocationId { get; set; }
        public string BuildingName { get; set; }
        public string YearBuilt { get; set; }
        public string SquareFootage { get; set; }

        public Address Address { get; set; }
    }
}
