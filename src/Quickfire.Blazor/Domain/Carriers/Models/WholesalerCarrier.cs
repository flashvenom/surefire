using System.ComponentModel.DataAnnotations;

namespace Quickfire.Blazor.Domain.Carriers.Models
{
    /// <summary>
    /// Junction table to track which carriers a wholesaler has access to
    /// </summary>
    public class WholesalerCarrier
    {
        [Key]
        public int WholesalerCarrierId { get; set; }
        
        public int WholesalerId { get; set; }
        public Carrier Wholesaler { get; set; }
        
        public int CarrierId { get; set; }
        public Carrier Carrier { get; set; }
        
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; }
        
        /// <summary>
        /// Optional notes about this wholesaler-carrier relationship
        /// </summary>
        public string? Notes { get; set; }
        
        /// <summary>
        /// Whether this relationship is currently active
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
} 