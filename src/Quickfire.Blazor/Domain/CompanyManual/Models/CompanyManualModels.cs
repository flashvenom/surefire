using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.Renewals.Models;

namespace Quickfire.Blazor.Domain.CompanyManual.Models
{
    public class CompanyManualPage
    {
        public int CompanyManualPageId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string? ProcedureType { get; set; }
        public string? LineOfBusiness { get; set; }
        public string? SlaTarget { get; set; }
        public string? OwnerUserId { get; set; }
        public ApplicationUser? OwnerUser { get; set; }
        public string? Tags { get; set; }
        public string? Keywords { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ReviewBy { get; set; }
        public int? ReviewCycleDays { get; set; }
        public int? TaskGroupId { get; set; }
        public TaskGroup? TaskGroup { get; set; }

        public int? ParentPageId { get; set; }
        public CompanyManualPage? ParentPage { get; set; }
        public ICollection<CompanyManualPage> ChildPages { get; set; } = new List<CompanyManualPage>();
        public int SortOrder { get; set; }
        public bool IsArchived { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public string? UpdatedById { get; set; }
        public ApplicationUser? UpdatedBy { get; set; }
        public DateTime? ArchivedAt { get; set; }
        public string? ArchivedById { get; set; }
        public ApplicationUser? ArchivedBy { get; set; }

        public int? PublishedRevisionId { get; set; }
        public CompanyManualRevision? PublishedRevision { get; set; }
        public ICollection<CompanyManualRevision> Revisions { get; set; } = new List<CompanyManualRevision>();
        public ICollection<CompanyManualSuggestion> Suggestions { get; set; } = new List<CompanyManualSuggestion>();
    }

    public class CompanyManualRevision
    {
        public int CompanyManualRevisionId { get; set; }
        public int CompanyManualPageId { get; set; }
        public CompanyManualPage Page { get; set; } = null!;
        public int RevisionNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ContentHtml { get; set; } = string.Empty;
        public string ContentText { get; set; } = string.Empty;
        public string? ChangeSummary { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string? PublishedById { get; set; }
        public ApplicationUser? PublishedBy { get; set; }
        public int? SourceSuggestionId { get; set; }
        public CompanyManualSuggestion? SourceSuggestion { get; set; }
    }

    public class CompanyManualSuggestion
    {
        public int CompanyManualSuggestionId { get; set; }
        public int CompanyManualPageId { get; set; }
        public CompanyManualPage Page { get; set; } = null!;
        public int? BasedOnRevisionId { get; set; }
        public CompanyManualRevision? BasedOnRevision { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ContentHtml { get; set; } = string.Empty;
        public string ContentText { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public CompanyManualSuggestionStatus Status { get; set; } = CompanyManualSuggestionStatus.Pending;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public string? SubmittedById { get; set; }
        public ApplicationUser? SubmittedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewedById { get; set; }
        public ApplicationUser? ReviewedBy { get; set; }
        public string? ReviewNotes { get; set; }
        public int? OutcomeRevisionId { get; set; }
        public CompanyManualRevision? OutcomeRevision { get; set; }
    }

    public class CompanyManualAuditEntry
    {
        public int CompanyManualAuditEntryId { get; set; }
        public int? CompanyManualPageId { get; set; }
        public CompanyManualPage? Page { get; set; }
        public int? CompanyManualRevisionId { get; set; }
        public CompanyManualRevision? Revision { get; set; }
        public int? CompanyManualSuggestionId { get; set; }
        public CompanyManualSuggestion? Suggestion { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? DetailsJson { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? ActorUserId { get; set; }
        public ApplicationUser? ActorUser { get; set; }
    }

    public sealed class CompanyManualSearchResult
    {
        public int PageId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Snippet { get; set; }
        public string? ProcedureType { get; set; }
        public string? LineOfBusiness { get; set; }
    }

    public enum CompanyManualSuggestionStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }
}
