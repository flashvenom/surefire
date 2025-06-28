using System;
using Surefire.Domain.Clients.Models;
using Surefire.Domain.Renewals.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Surefire.Domain.Forms.Models
{
    [Table("EmailTemplates")]
    public class EmailTemplate
    {
        [Key]
        public int EmailTemplateId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool NeedsContact { get; set; } = true;
        public bool NeedsPolicy { get; set; } = true;
        public bool NeedsProduct { get; set; } = false;
        public bool NeedsPayment { get; set; } = false;
        public bool NeedsDownPayment { get; set; } = false;

        [StringLength(50)]
        public string? CustomFunction { get; set; }

        public EmailTemplate()
        {
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }
    }
}
