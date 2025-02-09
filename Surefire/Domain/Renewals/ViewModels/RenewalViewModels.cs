using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Surefire.Domain.Clients.Models;
using Surefire.Domain.Renewals.Models;
using Surefire.Domain.Carriers.Models;
using Surefire.Domain.Policies.Models;
using Surefire.Domain.Shared;
using Surefire.Data;

namespace Surefire.Domain.Renewals.ViewModels
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

    public class RenewalEditViewModel
    {
        public int RenewalId { get; set; }
        public string? PolicyNumber { get; set; }
        public decimal? ExpiringPremium { get; set; }
        public DateTime RenewalDate { get; set; }
        public string Status { get; set; }
        public string AssignedToId { get; set; }
        public int ProductId { get; set; }
        public int CarrierId { get; set; }
        public int WholesalerId { get; set; }
        public int ClientId { get; set; }
        public string Notes { get; set; }

        public List<UserViewModel> Users { get; set; } = new();
        public List<ProductViewModel> Products { get; set; } = new();
        public List<CarrierViewModel> Carriers { get; set; } = new();
        public List<WholesalerViewModel> Wholesalers { get; set; } = new();
        public List<ClientViewModel> Clients { get; set; } = new();
    }

    public class UserViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
    }

    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }

    public class CarrierViewModel
    {
        public int CarrierId { get; set; }
        public string CarrierName { get; set; }
    }

    public class WholesalerViewModel
    {
        public int WholesalerId { get; set; }
        public string WholesalerName { get; set; }
    }

    public class ClientViewModel
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
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
        public DateTime? TaskGoalDate { get; set; }
        public DateTime? TaskCompletedDate { get; set; }
        public ApplicationUser AssignedSubUser { get; set; }
    }
}
