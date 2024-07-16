using System.ComponentModel.DataAnnotations;

namespace Mantis.Domain.Renewals.ViewModels
{
    public class SubmissionViewModel
    {
        public int Id { get; set; }
        public int MarketingEntryId { get; set; }

        [Required]
        public string WholesalerName { get; set; }

        [Required]
        public string CarrierName { get; set; }

        public string UnderwriterName { get; set; } = string.Empty;
        public string UnderwriterEmail { get; set; } = string.Empty;
        public decimal? QuotePrice { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string Status { get; set; } = "Gathering Info";
    }

    public class NoteViewModel
    {
        [Required]
        public int Id { get; set; }  // The ID of the submission
        [Required]
        public string NoteText { get; set; }
    }
    public class UpdateStatusModel
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }
}
