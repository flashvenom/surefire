using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.Forms.Models;
using Quickfire.Blazor.Domain.Shared.Helpers;
using Quickfire.Blazor.Domain.Shared.Services;
using Syncfusion.Pdf.Parsing;

namespace Quickfire.Blazor.Domain.Forms.Services
{
    public sealed class FormsLibraryService
    {
        private const long MaxUploadSize = 1024L * 1024L * 200L;
        private const int LibraryThumbnailWidth = 700;
        private static readonly string[] AllowedExtensions = { ".pdf" };
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly StateService _stateService;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FormsLibraryService> _logger;

        public FormsLibraryService(
            IDbContextFactory<ApplicationDbContext> dbContextFactory,
            StateService stateService,
            IWebHostEnvironment environment,
            ILogger<FormsLibraryService> logger)
        {
            _dbContextFactory = dbContextFactory;
            _stateService = stateService;
            _environment = environment;
            _logger = logger;
        }

        public async Task<List<FormsLibraryEntry>> GetLibraryAsync(CancellationToken cancellationToken = default)
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await context.FormsLibrary
                .Include(entry => entry.ActiveVersion)
                .AsNoTracking()
                .OrderBy(entry => entry.Title ?? string.Empty)
                .ToListAsync(cancellationToken);
        }

        public async Task<FormsLibraryEntry?> GetEntryAsync(int entryId, bool includeVersions, CancellationToken cancellationToken = default)
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            IQueryable<FormsLibraryEntry> query = context.FormsLibrary
                .Include(entry => entry.ActiveVersion);

            if (includeVersions)
            {
                query = query.Include(entry => entry.Versions);
            }

            return await query.AsNoTracking()
                .FirstOrDefaultAsync(entry => entry.FormsLibraryEntryId == entryId, cancellationToken);
        }

        public async Task<IReadOnlyList<FormsLibraryVersion>> GetVersionsAsync(int entryId, CancellationToken cancellationToken = default)
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await context.FormsLibraryVersions
                .Where(version => version.FormsLibraryEntryId == entryId)
                .OrderByDescending(version => version.VersionNumber)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<FormsLibraryEntry> CreateEntryAsync(
            IBrowserFile file,
            FormsLibraryEntry draft,
            string? versionLabel,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(file);
            ArgumentNullException.ThrowIfNull(draft);

            ValidateUpload(file);

            await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var currentUser = _stateService.CurrentUser;
            if (currentUser != null)
            {
                context.Attach(currentUser);
            }

            var now = DateTime.UtcNow;
            var entry = new FormsLibraryEntry
            {
                Title = NormalizeText(draft.Title) ?? Path.GetFileNameWithoutExtension(file.Name),
                CarrierName = NormalizeText(draft.CarrierName),
                WholesalerName = NormalizeText(draft.WholesalerName),
                MarketTag = NormalizeText(draft.MarketTag),
                Rating = NormalizeRating(draft.Rating),
                IsBookmarked = draft.IsBookmarked,
                IsArchived = false,
                DateCreated = now,
                DateModified = now,
                CreatedBy = currentUser,
                ModifiedBy = currentUser
            };

            context.FormsLibrary.Add(entry);
            await context.SaveChangesAsync(cancellationToken);

            var version = await CreateVersionAsync(context, entry, file, versionLabel, currentUser, cancellationToken);
            entry.ActiveVersionId = version.FormsLibraryVersionId;
            entry.DateModified = DateTime.UtcNow;

            context.FormsLibrary.Update(entry);
            await context.SaveChangesAsync(cancellationToken);

            return entry;
        }

        public async Task<FormsLibraryVersion> AddVersionAsync(
            int entryId,
            IBrowserFile file,
            string? versionLabel,
            bool setActive,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(file);
            ValidateUpload(file);

            await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var entry = await context.FormsLibrary
                .FirstOrDefaultAsync(e => e.FormsLibraryEntryId == entryId, cancellationToken);

            if (entry == null)
            {
                throw new InvalidOperationException("Library entry not found.");
            }

            var currentUser = _stateService.CurrentUser;
            if (currentUser != null)
            {
                context.Attach(currentUser);
            }

            var version = await CreateVersionAsync(context, entry, file, versionLabel, currentUser, cancellationToken);

            if (setActive)
            {
                entry.ActiveVersionId = version.FormsLibraryVersionId;
                entry.DateModified = DateTime.UtcNow;
                entry.ModifiedBy = currentUser;
                context.FormsLibrary.Update(entry);
            }

            await context.SaveChangesAsync(cancellationToken);
            return version;
        }

        public async Task UpdateEntryAsync(FormsLibraryEntry entry, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entry);

            await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var existing = await context.FormsLibrary
                .FirstOrDefaultAsync(e => e.FormsLibraryEntryId == entry.FormsLibraryEntryId, cancellationToken);

            if (existing == null)
            {
                throw new InvalidOperationException("Library entry not found.");
            }

            var currentUser = _stateService.CurrentUser;
            if (currentUser != null)
            {
                context.Attach(currentUser);
            }

            existing.Title = NormalizeText(entry.Title);
            existing.CarrierName = NormalizeText(entry.CarrierName);
            existing.WholesalerName = NormalizeText(entry.WholesalerName);
            existing.MarketTag = NormalizeText(entry.MarketTag);
            existing.Rating = NormalizeRating(entry.Rating);
            existing.IsBookmarked = entry.IsBookmarked;
            existing.IsArchived = entry.IsArchived;
            existing.DateModified = DateTime.UtcNow;
            existing.ModifiedBy = currentUser;

            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task SetActiveVersionAsync(int entryId, int versionId, CancellationToken cancellationToken = default)
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var entry = await context.FormsLibrary
                .FirstOrDefaultAsync(e => e.FormsLibraryEntryId == entryId, cancellationToken);

            if (entry == null)
            {
                throw new InvalidOperationException("Library entry not found.");
            }

            var version = await context.FormsLibraryVersions
                .FirstOrDefaultAsync(v => v.FormsLibraryVersionId == versionId && v.FormsLibraryEntryId == entryId, cancellationToken);

            if (version == null)
            {
                throw new InvalidOperationException("Library version not found.");
            }

            var currentUser = _stateService.CurrentUser;
            if (currentUser != null)
            {
                context.Attach(currentUser);
            }

            entry.ActiveVersionId = version.FormsLibraryVersionId;
            entry.DateModified = DateTime.UtcNow;
            entry.ModifiedBy = currentUser;

            await context.SaveChangesAsync(cancellationToken);
        }

        public string? ResolveAbsolutePath(FormsLibraryVersion? version)
        {
            if (version == null ||
                string.IsNullOrWhiteSpace(version.RelativePath) ||
                string.IsNullOrWhiteSpace(version.StoredFileName))
            {
                return null;
            }

            var relative = version.RelativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
            return Path.Combine(_environment.WebRootPath, relative, version.StoredFileName);
        }

        private async Task<FormsLibraryVersion> CreateVersionAsync(
            ApplicationDbContext context,
            FormsLibraryEntry entry,
            IBrowserFile file,
            string? versionLabel,
            ApplicationUser? currentUser,
            CancellationToken cancellationToken)
        {
            var nextVersion = await context.FormsLibraryVersions
                .Where(v => v.FormsLibraryEntryId == entry.FormsLibraryEntryId)
                .Select(v => (int?)v.VersionNumber)
                .MaxAsync(cancellationToken) ?? 0;
            nextVersion += 1;

            var (storedFileName, relativePath, absolutePath) = await SaveFileAsync(entry.FormsLibraryEntryId, file, cancellationToken);
            var (pages, fields) = TryReadPdfMetadata(absolutePath);

            var version = new FormsLibraryVersion
            {
                FormsLibraryEntryId = entry.FormsLibraryEntryId,
                VersionNumber = nextVersion,
                VersionLabel = NormalizeText(versionLabel) ?? $"Version {nextVersion}",
                OriginalFileName = file.Name,
                StoredFileName = storedFileName,
                RelativePath = relativePath,
                FileSize = file.Size,
                Pages = pages,
                InteractiveElements = fields,
                FormFieldCount = fields,
                UploadedAt = DateTime.UtcNow,
                UploadedBy = currentUser
            };

            context.FormsLibraryVersions.Add(version);
            await context.SaveChangesAsync(cancellationToken);

            await TryGenerateThumbnailAsync(absolutePath, storedFileName, relativePath, cancellationToken);
            return version;
        }

        private static void ValidateUpload(IBrowserFile file)
        {
            var extension = Path.GetExtension(file.Name) ?? string.Empty;
            if (!AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Only PDF files are supported for the library.");
            }

            if (file.Size <= 0 || file.Size > MaxUploadSize)
            {
                throw new InvalidOperationException("File is empty or exceeds the 200 MB limit.");
            }
        }

        private async Task<(string storedFileName, string relativePath, string absolutePath)> SaveFileAsync(
            int entryId,
            IBrowserFile file,
            CancellationToken cancellationToken)
        {
            var folderName = entryId.ToString();
            var relativePath = Path.Combine("uploads", "library", folderName);
            var absoluteFolder = Path.Combine(_environment.WebRootPath, relativePath);
            Directory.CreateDirectory(absoluteFolder);

            var sanitizedBaseName = SanitizeFileName(Path.GetFileNameWithoutExtension(file.Name));
            var extension = Path.GetExtension(file.Name);
            var hash = StringHelper.GenerateFiveCharacterHash($"{sanitizedBaseName}_{DateTime.UtcNow.Ticks}");
            var storedFileName = $"{sanitizedBaseName}_{hash}{extension}";
            var absolutePath = Path.Combine(absoluteFolder, storedFileName);

            await using var source = file.OpenReadStream(MaxUploadSize);
            await using var destination = new FileStream(absolutePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await source.CopyToAsync(destination, cancellationToken);

            return (storedFileName, relativePath.Replace("\\", "/"), absolutePath);
        }

        private static string SanitizeFileName(string? baseName)
        {
            if (string.IsNullOrWhiteSpace(baseName))
            {
                return "library-form";
            }

            var invalidChars = Path.GetInvalidFileNameChars();
            var cleaned = new string(baseName.Where(ch => !invalidChars.Contains(ch)).ToArray());
            return string.IsNullOrWhiteSpace(cleaned) ? "library-form" : cleaned;
        }

        private static (int? pages, int? fields) TryReadPdfMetadata(string absolutePath)
        {
            try
            {
                using var stream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var loadedDocument = new PdfLoadedDocument(stream);
                var pageCount = loadedDocument.Pages?.Count ?? 0;
                var fieldCount = loadedDocument.Form?.Fields?.Count ?? 0;
                return (pageCount, fieldCount);
            }
            catch
            {
                return (null, null);
            }
        }

        private async Task TryGenerateThumbnailAsync(
            string absolutePath,
            string storedFileName,
            string relativePath,
            CancellationToken cancellationToken)
        {
            try
            {
                var baseFolder = Path.Combine(_environment.WebRootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
                var dataFolder = Path.Combine(baseFolder, ".data");
                Directory.CreateDirectory(dataFolder);
                var thumbName = Path.GetFileNameWithoutExtension(storedFileName) + "_thumb.jpg";
                var thumbPath = Path.Combine(dataFolder, thumbName);
                await PdfThumbnailGenerator.CreateJpegThumbnailAsync(
                    absolutePath,
                    thumbPath,
                    thumbnailWidth: LibraryThumbnailWidth,
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Thumbnail generation failed for library upload.");
            }
        }

        private static string? NormalizeText(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static int? NormalizeRating(int? rating)
        {
            if (!rating.HasValue || rating.Value < 1)
            {
                return null;
            }

            return rating.Value > 5 ? 5 : rating.Value;
        }
    }
}
