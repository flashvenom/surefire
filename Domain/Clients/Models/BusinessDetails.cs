using Mantis.Data;
using Mantis.Domain.Policies.Models;
using Mantis.Domain.Contacts.Models;
using Mantis.Domain.Shared;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Mantis.Domain.Clients.Models
{
    public class BusinessDetails
    {
        public int BusinessDetailsId { get; set; }
        public Client Client { get; set; }
        public int ClientId { get; set; }
        public string RecordType { get; set; } = "orphan";
        //
        public DateTime DateModified { get; set; } = DateTime.UtcNow;
        public string? LegalEntityType { get; set; }
        public string? BusinessShortDescription { get; set; }
        public string? BusinessLongDescription { get; set; }
        public DateTime? DateStarted { get; set; }
        public int? YearsExperience { get; set; }
        public string? FEIN { get; set; }
        public string? IndustryCategory { get; set; }
        public string? NatureOfBusiness { get; set; }
        public string? IndustrySpecialty { get; set; }
        public string? LicenseType { get; set; }
        public string? LicenseNumber { get; set; }
        public int? NumPartTimeEmployees { get; set; }
        public int? NumFullTimeEmployees { get; set; }
        public decimal? EstimatedGrossSales { get; set; }
        public int? NumClaims { get; set; }
    }
    public enum LegalEntityType
    {
        Individual,
        Corporation,
        SCorporation,
        Nonprofit,
        LLC,
        Partnership,
        Trust,
        JointVenture
    }

    public enum NatureOfBusiness
    {
        RealEstate,
        Contractor,
        Wholesale,
        Retail,
        Manufacturing,
        Office,
        Service,
        Resteraunt,
        Hospitality,
        Other
    }
    

}
