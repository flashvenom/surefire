using Surefire.Domain.Policies.Models;

namespace Surefire.Domain.Shared.Models
{
    public class Vehicle
    {
        public int VehicleId { get; set; }
        public string Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string VIN { get; set; }
        public string LicensePlate { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string CountryOfRegistration { get; set; }

    }

}
