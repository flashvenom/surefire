using Quickfire.Blazor.Data;

namespace Quickfire.Blazor.Domain.Forms.Models
{
    public sealed class FormsLibraryEntry
    {
        public int FormsLibraryEntryId { get; set; }
        public string? Title { get; set; }
        public string? CarrierName { get; set; }
        public string? WholesalerName { get; set; }
        public string? MarketTag { get; set; }
        public int? Rating { get; set; }
        public bool IsBookmarked { get; set; }
        public bool IsArchived { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public string? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public string? ModifiedById { get; set; }
        public ApplicationUser? ModifiedBy { get; set; }

        public int? ActiveVersionId { get; set; }
        public FormsLibraryVersion? ActiveVersion { get; set; }
        public ICollection<FormsLibraryVersion> Versions { get; set; } = new List<FormsLibraryVersion>();
    }

    public sealed class FormsLibraryVersion
    {
        public int FormsLibraryVersionId { get; set; }
        public int FormsLibraryEntryId { get; set; }
        public FormsLibraryEntry? Entry { get; set; }
        public int VersionNumber { get; set; }
        public string? VersionLabel { get; set; }
        public string? OriginalFileName { get; set; }
        public string? StoredFileName { get; set; }
        public string? RelativePath { get; set; }
        public long? FileSize { get; set; }
        public int? Pages { get; set; }
        public int? InteractiveElements { get; set; }
        public int? FormFieldCount { get; set; }
        public string? JsonFields { get; set; }
        public DateTime UploadedAt { get; set; }
        public string? UploadedById { get; set; }
        public ApplicationUser? UploadedBy { get; set; }
    }
}
