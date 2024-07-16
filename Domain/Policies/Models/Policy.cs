using Folient.Domain.Shared;
using System.Collections.Generic;
using System;
using Folient.Domain.Carriers.Models;
using Folient.Domain.Renewals.Models;

namespace Folient.Domain.Policies.Models
{
    public class Policy
    {
        public int PolicyId { get; set; }
        public string PolicyNumber { get; set; }
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
