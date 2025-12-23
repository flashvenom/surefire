using System.ComponentModel.DataAnnotations;
using Quickfire.Blazor.Domain.Contacts.Models;

namespace Quickfire.Blazor.Domain.Contacts.ViewModels
{
    public class ContactCreateViewModel
    {
        // Basic Information
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        // Phone Information
        public List<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>();

        // Email Information
        public List<EmailAddress> EmailAddresses { get; set; } = new List<EmailAddress>();

        // Association
        public string AssociationType { get; set; } = string.Empty; // "Client" or "Carrier"
        public int? ClientId { get; set; }
        public int? CarrierId { get; set; }

        // Carrier-specific fields
        public bool Underwriter { get; set; }
        public bool Service { get; set; }
        public bool Billing { get; set; }
        public bool Representative { get; set; }
    }
} 