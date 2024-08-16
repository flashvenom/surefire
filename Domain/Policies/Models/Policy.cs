﻿using Mantis.Domain.Shared;
using Mantis.Domain.Carriers.Models;
using Mantis.Domain.Clients.Models;
using Mantis.Domain.Renewals.Models;
using System.Text.Json.Serialization;
using Mantis.Data;

namespace Mantis.Domain.Policies.Models
{
    public class Policy
    {
        public int PolicyId { get; set; }
        public string PolicyNumber { get; set; }
        public string? ePolicyId { get; set; }
        public string? ePolicyLineId { get; set; }
        public string? eType { get; set; }
        public string? eTypeCode { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public decimal Premium { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; } = DateTime.UtcNow;

        public Application? Application { get; set; }
        public Carrier? Carrier { get; set; }
        public int? CarrierId { get; set; }
        public Carrier? Wholesaler { get; set; }
        public int? WholesalerId { get; set; }
        public Product? Product { get; set; }
        public int? ProductId { get; set; }

        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
        public ICollection<Driver> Drivers { get; set; } = new List<Driver>();
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public ICollection<Renewal> Renewals { get; set; } = new List<Renewal>();

        // Foreign keys
        public int ClientId { get; set; }
        [JsonIgnore]
        public Client Client { get; set; }
        public ApplicationUser? Producer { get; set; }
        public string? CSRId { get; set; }
        public ApplicationUser? CSR { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
    }

    public class PolicyDto
    {
        public int PolicyId { get; set; }
        public string PolicyNumber { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public decimal Premium { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public string ClientName { get; set; } // Include only the necessary client data
        public string LineType { get; set; } // Include only the necessary client data
        public string CarrierName { get; set; }
        public string WholesalerName { get; set; }
        public string ProducerName { get; set; }
    }
}
