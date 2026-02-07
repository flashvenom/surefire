using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.CompanyManual.Models;
using Quickfire.Blazor.Domain.Shared.Services;

namespace Quickfire.Blazor.Domain.CompanyManual.Services
{
    public class CompanyManualService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly StateService _stateService;

        public CompanyManualService(IDbContextFactory<ApplicationDbContext> dbContextFactory, StateService stateService)
        {
            _dbContextFactory = dbContextFactory;
            _stateService = stateService;
        }

        public async Task<List<CompanyManualPage>> GetPagesAsync(bool includeArchived = false)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var query = context.CompanyManualPages
                .AsNoTracking()
                .OrderBy(p => p.SortOrder)
                .ThenBy(p => p.Title);

            if (!includeArchived)
            {
                return await query.Where(p => !p.IsArchived).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public async Task<List<CompanyManualPage>> GetPagesWithPublishedRevisionsAsync(bool includeArchived = false)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var query = context.CompanyManualPages
                .Include(p => p.PublishedRevision)
                .AsNoTracking()
                .OrderBy(p => p.SortOrder)
                .ThenBy(p => p.Title);

            if (!includeArchived)
            {
                return await query.Where(p => !p.IsArchived).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public async Task<CompanyManualPage?> GetPageAsync(int pageId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.CompanyManualPages
                .Include(p => p.PublishedRevision)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.CompanyManualPageId == pageId);
        }

        public async Task<CompanyManualPage?> GetPageBySlugAsync(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return null;
            }

            using var context = _dbContextFactory.CreateDbContext();
            return await context.CompanyManualPages
                .Include(p => p.PublishedRevision)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Slug == slug);
        }

        public async Task<List<CompanyManualRevision>> GetRevisionsAsync(int pageId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.CompanyManualRevisions
                .AsNoTracking()
                .Where(r => r.CompanyManualPageId == pageId)
                .OrderByDescending(r => r.RevisionNumber)
                .ToListAsync();
        }

        public async Task<List<CompanyManualSuggestion>> GetSuggestionsAsync(int pageId, CompanyManualSuggestionStatus? status = null)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var query = context.CompanyManualSuggestions
                .AsNoTracking()
                .Where(s => s.CompanyManualPageId == pageId);

            if (status.HasValue)
            {
                query = query.Where(s => s.Status == status.Value);
            }

            return await query
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();
        }

        public async Task<List<CompanyManualSuggestion>> GetPendingSuggestionsAsync()
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.CompanyManualSuggestions
                .AsNoTracking()
                .Where(s => s.Status == CompanyManualSuggestionStatus.Pending)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();
        }

        public async Task<CompanyManualPage> CreatePageAsync(string title, int? parentPageId, string? actorUserId)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title is required.", nameof(title));
            }

            using var context = _dbContextFactory.CreateDbContext();
            var baseSlug = Slugify(title);
            var slug = await EnsureUniqueSlugAsync(context, baseSlug, null);
            var sortOrder = await GetNextSortOrderAsync(context, parentPageId);

            var page = new CompanyManualPage
            {
                Title = title.Trim(),
                Slug = slug,
                ParentPageId = parentPageId,
                SortOrder = sortOrder,
                CreatedAt = DateTime.UtcNow,
                CreatedById = actorUserId
            };

            context.CompanyManualPages.Add(page);
            await context.SaveChangesAsync();
            AddAuditEntry(context, page.CompanyManualPageId, null, null, "PageCreated", actorUserId, new
            {
                page.Title,
                page.ParentPageId
            });
            await context.SaveChangesAsync();
            return page;
        }

        public async Task<bool> UpdatePageMetadataAsync(CompanyManualPage updated, string? actorUserId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var page = await context.CompanyManualPages.FirstOrDefaultAsync(p => p.CompanyManualPageId == updated.CompanyManualPageId);
            if (page == null)
            {
                return false;
            }

            page.Summary = updated.Summary?.Trim();
            page.ProcedureType = updated.ProcedureType?.Trim();
            page.LineOfBusiness = updated.LineOfBusiness?.Trim();
            page.SlaTarget = updated.SlaTarget?.Trim();
            page.OwnerUserId = string.IsNullOrWhiteSpace(updated.OwnerUserId) ? null : updated.OwnerUserId;
            page.Tags = updated.Tags?.Trim();
            page.Keywords = updated.Keywords?.Trim();
            page.EffectiveDate = updated.EffectiveDate;
            page.ReviewBy = updated.ReviewBy;
            page.ReviewCycleDays = updated.ReviewCycleDays;
            page.TaskGroupId = updated.TaskGroupId;
            page.UpdatedAt = DateTime.UtcNow;
            page.UpdatedById = actorUserId;

            AddAuditEntry(context, page.CompanyManualPageId, null, null, "PageMetadataUpdated", actorUserId, new
            {
                page.ProcedureType,
                page.LineOfBusiness,
                page.SlaTarget,
                page.OwnerUserId,
                page.TaskGroupId
            });

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePageParentAsync(int pageId, int? newParentId, string? actorUserId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var page = await context.CompanyManualPages.FirstOrDefaultAsync(p => p.CompanyManualPageId == pageId);
            if (page == null)
            {
                return false;
            }

            if (newParentId == page.CompanyManualPageId)
            {
                return false;
            }

            if (page.ParentPageId == newParentId)
            {
                return true;
            }

            if (await IsDescendantAsync(context, newParentId, page.CompanyManualPageId))
            {
                return false;
            }

            page.ParentPageId = newParentId;
            page.SortOrder = await GetNextSortOrderAsync(context, newParentId);
            page.UpdatedAt = DateTime.UtcNow;
            page.UpdatedById = actorUserId;

            AddAuditEntry(context, page.CompanyManualPageId, null, null, "PageMoved", actorUserId, new
            {
                page.ParentPageId
            });

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MovePageOrderAsync(int pageId, bool moveUp, string? actorUserId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var page = await context.CompanyManualPages.FirstOrDefaultAsync(p => p.CompanyManualPageId == pageId);
            if (page == null)
            {
                return false;
            }

            var siblings = await context.CompanyManualPages
                .Where(p => p.ParentPageId == page.ParentPageId)
                .OrderBy(p => p.SortOrder)
                .ThenBy(p => p.CompanyManualPageId)
                .ToListAsync();

            var index = siblings.FindIndex(p => p.CompanyManualPageId == pageId);
            if (index < 0)
            {
                return false;
            }

            var swapIndex = moveUp ? index - 1 : index + 1;
            if (swapIndex < 0 || swapIndex >= siblings.Count)
            {
                return false;
            }

            var other = siblings[swapIndex];
            var temp = page.SortOrder;
            page.SortOrder = other.SortOrder;
            other.SortOrder = temp;

            page.UpdatedAt = DateTime.UtcNow;
            page.UpdatedById = actorUserId;
            other.UpdatedAt = DateTime.UtcNow;
            other.UpdatedById = actorUserId;

            AddAuditEntry(context, page.CompanyManualPageId, null, null, "PageReordered", actorUserId, new
            {
                Direction = moveUp ? "Up" : "Down"
            });

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ArchivePageAsync(int pageId, string? actorUserId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var page = await context.CompanyManualPages.FirstOrDefaultAsync(p => p.CompanyManualPageId == pageId);
            if (page == null || page.IsArchived)
            {
                return false;
            }

            page.IsArchived = true;
            page.ArchivedAt = DateTime.UtcNow;
            page.ArchivedById = actorUserId;
            page.UpdatedAt = DateTime.UtcNow;
            page.UpdatedById = actorUserId;

            AddAuditEntry(context, page.CompanyManualPageId, null, null, "PageArchived", actorUserId, null);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestorePageAsync(int pageId, string? actorUserId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var page = await context.CompanyManualPages.FirstOrDefaultAsync(p => p.CompanyManualPageId == pageId);
            if (page == null || !page.IsArchived)
            {
                return false;
            }

            page.IsArchived = false;
            page.ArchivedAt = null;
            page.ArchivedById = null;
            page.UpdatedAt = DateTime.UtcNow;
            page.UpdatedById = actorUserId;

            AddAuditEntry(context, page.CompanyManualPageId, null, null, "PageRestored", actorUserId, null);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<CompanyManualRevision?> PublishRevisionAsync(int pageId, string title, string contentHtml, string? changeSummary, string? actorUserId, int? sourceSuggestionId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var page = await context.CompanyManualPages.FirstOrDefaultAsync(p => p.CompanyManualPageId == pageId);
            if (page == null)
            {
                return null;
            }

            var revisionNumber = await GetNextRevisionNumberAsync(context, pageId);
            var normalizedTitle = string.IsNullOrWhiteSpace(title) ? page.Title : title.Trim();
            var revision = new CompanyManualRevision
            {
                CompanyManualPageId = pageId,
                RevisionNumber = revisionNumber,
                Title = normalizedTitle,
                ContentHtml = contentHtml ?? string.Empty,
                ContentText = NormalizeHtmlToText(contentHtml),
                ChangeSummary = changeSummary?.Trim(),
                CreatedAt = DateTime.UtcNow,
                CreatedById = actorUserId,
                PublishedAt = DateTime.UtcNow,
                PublishedById = actorUserId,
                SourceSuggestionId = sourceSuggestionId
            };

            context.CompanyManualRevisions.Add(revision);
            page.Title = normalizedTitle;
            page.PublishedRevision = revision;
            page.UpdatedAt = DateTime.UtcNow;
            page.UpdatedById = actorUserId;

            await context.SaveChangesAsync();
            AddAuditEntry(context, page.CompanyManualPageId, revision.CompanyManualRevisionId, null, "RevisionPublished", actorUserId, new
            {
                revision.RevisionNumber,
                revision.Title
            });
            await context.SaveChangesAsync();
            return revision;
        }

        public async Task<CompanyManualSuggestion?> SubmitSuggestionAsync(int pageId, string title, string contentHtml, string? summary, string? actorUserId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var page = await context.CompanyManualPages.AsNoTracking().FirstOrDefaultAsync(p => p.CompanyManualPageId == pageId);
            if (page == null)
            {
                return null;
            }

            var suggestion = new CompanyManualSuggestion
            {
                CompanyManualPageId = pageId,
                BasedOnRevisionId = page.PublishedRevisionId,
                Title = string.IsNullOrWhiteSpace(title) ? page.Title : title.Trim(),
                ContentHtml = contentHtml ?? string.Empty,
                ContentText = NormalizeHtmlToText(contentHtml),
                Summary = summary?.Trim(),
                SubmittedAt = DateTime.UtcNow,
                SubmittedById = actorUserId,
                Status = CompanyManualSuggestionStatus.Pending
            };

            context.CompanyManualSuggestions.Add(suggestion);
            AddAuditEntry(context, pageId, null, null, "SuggestionSubmitted", actorUserId, new
            {
                suggestion.Title
            });
            await context.SaveChangesAsync();
            return suggestion;
        }

        public async Task<CompanyManualSuggestion?> ApproveSuggestionAsync(int suggestionId, string? reviewNotes, string? actorUserId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var suggestion = await context.CompanyManualSuggestions
                .Include(s => s.Page)
                .FirstOrDefaultAsync(s => s.CompanyManualSuggestionId == suggestionId);

            if (suggestion == null || suggestion.Status != CompanyManualSuggestionStatus.Pending)
            {
                return null;
            }

            var revision = await PublishRevisionAsyncInternal(context, suggestion, actorUserId);
            if (revision == null)
            {
                return null;
            }

            suggestion.Status = CompanyManualSuggestionStatus.Approved;
            suggestion.ReviewedAt = DateTime.UtcNow;
            suggestion.ReviewedById = actorUserId;
            suggestion.ReviewNotes = reviewNotes?.Trim();
            suggestion.OutcomeRevision = revision;

            await context.SaveChangesAsync();
            if (revision != null)
            {
                AddAuditEntry(context, suggestion.CompanyManualPageId, revision.CompanyManualRevisionId, suggestion.CompanyManualSuggestionId, "RevisionPublished", actorUserId, new
                {
                    revision.RevisionNumber,
                    revision.Title
                });
            }

            AddAuditEntry(context, suggestion.CompanyManualPageId, revision?.CompanyManualRevisionId, suggestion.CompanyManualSuggestionId, "SuggestionApproved", actorUserId, new
            {
                suggestion.Title
            });
            await context.SaveChangesAsync();
            return suggestion;
        }

        public async Task<CompanyManualSuggestion?> RejectSuggestionAsync(int suggestionId, string? reviewNotes, string? actorUserId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var suggestion = await context.CompanyManualSuggestions
                .FirstOrDefaultAsync(s => s.CompanyManualSuggestionId == suggestionId);

            if (suggestion == null || suggestion.Status != CompanyManualSuggestionStatus.Pending)
            {
                return null;
            }

            suggestion.Status = CompanyManualSuggestionStatus.Rejected;
            suggestion.ReviewedAt = DateTime.UtcNow;
            suggestion.ReviewedById = actorUserId;
            suggestion.ReviewNotes = reviewNotes?.Trim();

            await context.SaveChangesAsync();
            AddAuditEntry(context, suggestion.CompanyManualPageId, null, suggestion.CompanyManualSuggestionId, "SuggestionRejected", actorUserId, null);
            await context.SaveChangesAsync();
            return suggestion;
        }

        public async Task<List<CompanyManualSearchResult>> SearchAsync(string query, bool includeArchived = false)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<CompanyManualSearchResult>();
            }

            var search = query.Trim();
            var like = $"%{search}%";

            using var context = _dbContextFactory.CreateDbContext();
            var baseQuery = context.CompanyManualPages
                .AsNoTracking()
                .Include(p => p.PublishedRevision)
                .Where(p => p.PublishedRevisionId != null);

            if (!includeArchived)
            {
                baseQuery = baseQuery.Where(p => !p.IsArchived);
            }

            var matches = await baseQuery
                .Where(p => EF.Functions.Like(p.Title, like) || EF.Functions.Like(p.PublishedRevision!.ContentText, like))
                .OrderBy(p => p.Title)
                .Select(p => new CompanyManualSearchResult
                {
                    PageId = p.CompanyManualPageId,
                    Title = p.Title,
                    Slug = p.Slug,
                    ProcedureType = p.ProcedureType,
                    LineOfBusiness = p.LineOfBusiness,
                    Snippet = p.PublishedRevision!.ContentText
                })
                .ToListAsync();

            foreach (var match in matches)
            {
                match.Snippet = BuildSnippet(match.Snippet, search);
            }

            return matches;
        }

        public async Task<List<CompanyManualAuditEntry>> GetAuditEntriesAsync(int pageId, int take = 50)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.CompanyManualAuditEntries
                .AsNoTracking()
                .Where(a => a.CompanyManualPageId == pageId)
                .OrderByDescending(a => a.Timestamp)
                .Take(take)
                .ToListAsync();
        }

        public async Task<bool> IsCompanyManualAdminAsync(string? userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            var settings = await _stateService.GetSystemSettingsAsync();
            return settings != null &&
                   !string.IsNullOrWhiteSpace(settings.CompanyManualAdminUserId) &&
                   string.Equals(settings.CompanyManualAdminUserId, userId, StringComparison.Ordinal);
        }

        private async Task<CompanyManualRevision?> PublishRevisionAsyncInternal(ApplicationDbContext context, CompanyManualSuggestion suggestion, string? actorUserId)
        {
            var page = await context.CompanyManualPages.FirstOrDefaultAsync(p => p.CompanyManualPageId == suggestion.CompanyManualPageId);
            if (page == null)
            {
                return null;
            }

            var revisionNumber = await GetNextRevisionNumberAsync(context, page.CompanyManualPageId);
            var revision = new CompanyManualRevision
            {
                CompanyManualPageId = page.CompanyManualPageId,
                RevisionNumber = revisionNumber,
                Title = suggestion.Title,
                ContentHtml = suggestion.ContentHtml ?? string.Empty,
                ContentText = suggestion.ContentText ?? NormalizeHtmlToText(suggestion.ContentHtml),
                ChangeSummary = suggestion.Summary,
                CreatedAt = DateTime.UtcNow,
                CreatedById = actorUserId,
                PublishedAt = DateTime.UtcNow,
                PublishedById = actorUserId,
                SourceSuggestionId = suggestion.CompanyManualSuggestionId
            };

            context.CompanyManualRevisions.Add(revision);
            page.Title = revision.Title;
            page.PublishedRevision = revision;
            page.UpdatedAt = DateTime.UtcNow;
            page.UpdatedById = actorUserId;

            return revision;
        }

        private static void AddAuditEntry(
            ApplicationDbContext context,
            int? pageId,
            int? revisionId,
            int? suggestionId,
            string action,
            string? actorUserId,
            object? details)
        {
            var entry = new CompanyManualAuditEntry
            {
                CompanyManualPageId = pageId,
                CompanyManualRevisionId = revisionId,
                CompanyManualSuggestionId = suggestionId,
                Action = action,
                DetailsJson = details == null ? null : JsonSerializer.Serialize(details),
                ActorUserId = actorUserId,
                Timestamp = DateTime.UtcNow
            };

            context.CompanyManualAuditEntries.Add(entry);
        }

        private static async Task<int> GetNextRevisionNumberAsync(ApplicationDbContext context, int pageId)
        {
            var max = await context.CompanyManualRevisions
                .Where(r => r.CompanyManualPageId == pageId)
                .Select(r => (int?)r.RevisionNumber)
                .MaxAsync();
            return (max ?? 0) + 1;
        }

        private static async Task<int> GetNextSortOrderAsync(ApplicationDbContext context, int? parentPageId)
        {
            var max = await context.CompanyManualPages
                .Where(p => p.ParentPageId == parentPageId)
                .Select(p => (int?)p.SortOrder)
                .MaxAsync();
            return (max ?? -1) + 1;
        }

        private static async Task<bool> IsDescendantAsync(ApplicationDbContext context, int? candidateParentId, int pageId)
        {
            var currentId = candidateParentId;
            while (currentId.HasValue)
            {
                if (currentId.Value == pageId)
                {
                    return true;
                }

                currentId = await context.CompanyManualPages
                    .Where(p => p.CompanyManualPageId == currentId.Value)
                    .Select(p => p.ParentPageId)
                    .FirstOrDefaultAsync();
            }

            return false;
        }

        private static string Slugify(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "page";
            }

            var normalized = value.Trim().ToLowerInvariant();
            normalized = Regex.Replace(normalized, @"[^a-z0-9\s-]", string.Empty);
            normalized = Regex.Replace(normalized, @"\s+", " ").Trim();
            if (normalized.Length > 80)
            {
                normalized = normalized.Substring(0, 80);
            }
            normalized = normalized.Replace(" ", "-");
            normalized = Regex.Replace(normalized, "-+", "-");
            return string.IsNullOrWhiteSpace(normalized) ? "page" : normalized;
        }

        private static async Task<string> EnsureUniqueSlugAsync(ApplicationDbContext context, string baseSlug, int? pageId)
        {
            var slug = baseSlug;
            var counter = 1;

            while (await context.CompanyManualPages.AnyAsync(p => p.Slug == slug && p.CompanyManualPageId != pageId))
            {
                counter++;
                slug = $"{baseSlug}-{counter}";
            }

            return slug;
        }

        private static string NormalizeHtmlToText(string? html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            var withoutTags = Regex.Replace(html, "<[^>]+>", " ");
            var decoded = WebUtility.HtmlDecode(withoutTags);
            return Regex.Replace(decoded ?? string.Empty, @"\s+", " ").Trim();
        }

        private static string? BuildSnippet(string? text, string query)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            var normalized = Regex.Replace(text, @"\s+", " ").Trim();
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return null;
            }

            var index = normalized.IndexOf(query, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
            {
                return normalized.Length > 180 ? normalized.Substring(0, 180).Trim() + "..." : normalized;
            }

            var start = Math.Max(0, index - 60);
            var length = Math.Min(normalized.Length - start, 180);
            var snippet = normalized.Substring(start, length).Trim();

            if (start > 0)
            {
                snippet = "..." + snippet;
            }

            if (start + length < normalized.Length)
            {
                snippet += "...";
            }

            return snippet;
        }
    }
}
