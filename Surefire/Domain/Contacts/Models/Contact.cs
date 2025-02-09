﻿using Surefire.Domain.Shared.Models;
using Surefire.Domain.Clients.Models;
using Surefire.Domain.Carriers.Models;
using System.ComponentModel.DataAnnotations;

namespace Surefire.Domain.Contacts.Models
{
    public class Contact
    {
        public int ContactId { get; set; }
        [Required]
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string FullName => $"{FirstName ?? string.Empty} {LastName ?? string.Empty}".Trim();
        public string? Title { get; set; }
        public string? Email { get; set; }
        public string? EmailAlternate { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public string? Mobile { get; set; }
        public string? Notes { get; set; }
        public string? HeadshotFilename { get; set; }
        public bool IsStarred { get; set; } = false;
        public bool Underwriter { get; set; }
        public bool Service { get; set; }
        public bool Billing { get; set; }
        public bool Representative { get; set; }
        public bool Owner { get; set; }
        public bool IsInactive { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; } = DateTime.UtcNow;
        public Address? Address { get; set; }
        //Navigation Properties
        public int? ClientId { get; set; }
        public Client? Client { get; set; }
        public int? CarrierId { get; set; }
        public Carrier? Carrier { get; set; }
    }
    public class ContactDto
    {
        public int ContactId { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Mobile { get; set; }
        public string Notes { get; set; }
        public DateTime? DateCreated { get; set; }
        public string AssociatedWith { get; set; } // This property will hold either Client Name or Carrier Name
        public int AssociatedWithId { get; set; }
    }

}
