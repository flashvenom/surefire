using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Mantis.Domain.Clients.Models;

namespace Mantis.Domain.Renewals.ViewModels
{
    public class RenewalViewModel
    {
        public string? ExpiringPolicyNumber { get; set; }
        public int? WholesalerId { get; set; }
        public int? CarrierId { get; set; }
        public decimal ExpiringPremium { get; set; }
        [Required(ErrorMessage = "Please select a line of business.")]
        public int? ProductId { get; set; }
        [Required(ErrorMessage = "Enter an expiration date.")]
        [DataType(DataType.Date)]
        public DateTime RenewalDate { get; set; }
        [Required(ErrorMessage = "You must select a client.")]
        public int? ClientId { get; set; }
        [Required(ErrorMessage = "Please choose a user.")]
        public string? AssignedToId { get; set; }
    }

    public class TaskItemViewModel
    {
        public int TaskItemId { get; set; }
        public string TaskItemName { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsHighlighted { get; set; }
        public bool IsHidden { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}
