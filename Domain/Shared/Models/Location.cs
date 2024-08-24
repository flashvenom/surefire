namespace Mantis.Domain.Shared
{
    public class Location
    {
        public int LocationId { get; set; }
        public string BuildingName { get; set; }
        public string YearBuilt { get; set; }
        public string SquareFootage { get; set; }
        public bool? Owner { get; set; }
        public bool? Tenant { get; set; }
        public int? NumPartTimeEmployees { get; set; }
        public int? NumFullTimeEmployees { get; set; }
        public decimal? GrossSales { get; set; }
        public int? OccupiedSquareFootage { get; set; }
        public int? BuildingTotalSquareFootage { get; set; }
        public int? NumStories { get; set; }

        public Address Address { get; set; }
    }
}
