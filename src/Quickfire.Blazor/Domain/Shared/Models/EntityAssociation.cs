using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quickfire.Blazor.Domain.Shared.Models
{
    public class EntityAssociation
    {
        [Key]
        public int AssociationId { get; set; }

        // --- First Entity ---
        [Required]
        [MaxLength(50)] // e.g., "Client", "Contact"
        public string EntityType1 { get; set; }

        [Required]
        public int EntityId1 { get; set; }

        // --- Second Entity ---
        [Required]
        [MaxLength(50)]
        public string EntityType2 { get; set; }

        [Required]
        public int EntityId2 { get; set; }

        // --- Relationship Details ---
        [Required]
        [MaxLength(100)] // E.g., "Associated Client", "Spouse", "Son of", "Referred By"
        public string RelationshipDescription { get; set; }

        public string? Notes { get; set; } // Optional longer notes

        // --- Audit Info ---
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        // Optional: public string? CreatedById { get; set; }
    }
}