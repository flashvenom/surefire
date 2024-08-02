using Mantis.Domain.Shared;
using System.Collections.Generic;
using System;
using Mantis.Domain.Carriers.Models;
using Mantis.Domain.Renewals.Models;

namespace Mantis.Domain.Policies.Models
{
    public class Policy
    {
        //id to eTypeId
        //policyType.description to eType
        //policyType.code to eTypeCode
        //description to notes
        //effectiveOnto EffectiveDate
        //expirationOn to ExpirationDate
        //estimatedPremium.units concat estimatedPremium.partialUnits to Premium
        public int PolicyId { get; set; }
        public string PolicyNumber { get; set; }
        public string? ePolicyId { get; set; }
        public string? eType { get; set; }
        public string? eTypeCode { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public decimal Premium { get; set; }

        public Application Application { get; set; }
        public Carrier Carrier { get; set; }
        public Carrier Wholesaler { get; set; }
        public Product Product { get; set; }

        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
        public ICollection<Driver> Drivers { get; set; } = new List<Driver>();
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public ICollection<Renewal> Renewals { get; set; } = new List<Renewal>();
    }
}
