using System.ComponentModel.DataAnnotations;
using Quickfire.Blazor.Domain.Shared.Models;

namespace Quickfire.Blazor.Domain.Carriers.Models
{
    /// <summary>
    /// Junction table to track which products each carrier offers
    /// </summary>
    public class CarrierProduct
    {
        [Key]
        public int CarrierProductId { get; set; }
        
        public int CarrierId { get; set; }
        public Carrier Carrier { get; set; }
        
        public int ProductId { get; set; }
        public Product Product { get; set; }
        
        /// <summary>
        /// Indicates if this carrier is particularly good at this product line
        /// </summary>
        public bool ProductSpecialty { get; set; } = false;
        
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; }
        
        /// <summary>
        /// Optional notes about this carrier-product relationship
        /// </summary>
        public string? Notes { get; set; }
        
        /// <summary>
        /// Whether this relationship is currently active
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
} 