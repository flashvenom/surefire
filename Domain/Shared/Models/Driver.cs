using Mantis.Domain.Policies.Models;
using Mantis.Domain.Shared;
using System.Net;

namespace Mantis.Domain.Shared
{
    public class Driver
    {
        public int DriverId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime LicenseExpiryDate { get; set; }
        public bool IsPrimaryDriver { get; set; }

    }

}
