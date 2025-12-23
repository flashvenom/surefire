using System;
using System.Drawing;
using System.Collections.Generic;
using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.Attachments.Models;
using Quickfire.Blazor.Domain.Shared.Helpers;
using Quickfire.Blazor.Domain.Shared.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quickfire.Blazor.Domain.Logs;
using SixLabors.ImageSharp;
using Syncfusion.PdfToImageConverter;

namespace Quickfire.Blazor.Domain.Attachments.Services
{
    public class AttachmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly ILoggingService _log;
        private readonly StateService _stateService;

        public AttachmentService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, ILoggingService log, IDbContextFactory<ApplicationDbContext> contextFactory, StateService stateService)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _contextFactory = contextFactory;
            _log = log;
            _stateService = stateService;
        }

        //Headshots && Logos && Such
        public async Task<Attachment> AddPolicyAttachmentAsync(string fileName, int coverageId, string attachmentType)
        {
            switch (attachmentType.ToLower())
            {
                case "ai": // Additional Insured
                    var attachment = new Attachment
                    {
                        OriginalFileName = fileName,
                        Description = "Additional Insured Endorsements for GL Policy"
                    };

                    var coverage = await _context.GeneralLiabilityCoverages
                        .FirstOrDefaultAsync(c => c.GeneralLiabilityCoverageId == coverageId);

                    if (coverage == null) return null;

                    coverage.AdditionalInsuredAttachment = attachment;
                    _context.Attachments.Add(attachment);
                    await _context.SaveChangesAsync();
                    return attachment;
                case "wos": // Waiver of Subrogation
                    var attachmentwos = new Attachment
                    {
                        OriginalFileName = fileName,
                        Description = "Waiver Of Subrogation Endorsements for GL Policy"
                    };

                    var coveragewos = await _context.GeneralLiabilityCoverages
                        .FirstOrDefaultAsync(c => c.GeneralLiabilityCoverageId == coverageId);

                    if (coveragewos == null) return null;

                    coveragewos.WaiverOfSubAttachment = attachmentwos;
                    _context.Attachments.Add(attachmentwos);
                    await _context.SaveChangesAsync();
                    return attachmentwos;

                case "wc-wos": //WORK COMP  Waiver of Subrogation
                    var attachmentwcwos = new Attachment
                    {
                        OriginalFileName = fileName,
                        Description = "Waiver Of Subrogation Endorsements for WC Policy"
                    };

                    var coveragewcwos = await _context.WorkCompCoverages
                        .FirstOrDefaultAsync(c => c.WorkCompCoverageId == coverageId);

                    if (coveragewcwos == null) return null;

                    coveragewcwos.WaiverOfSubAttachment = attachmentwcwos;
                    _context.Attachments.Add(attachmentwcwos);
                    await _context.SaveChangesAsync();
                    return attachmentwcwos;
            }
            return null;
        }

        public async Task RemovePolicyAttachmentAsync(int coverageId, string attachmentType)
        {
            switch (attachmentType.ToLower())
            {
                case "gl-ai": // Additional Insured
                    var coverage = await _context.GeneralLiabilityCoverages
                    .Include(c => c.AdditionalInsuredAttachment)
                    .FirstOrDefaultAsync(c => c.GeneralLiabilityCoverageId == coverageId);

                    if (coverage == null) return;

                    if (coverage.AdditionalInsuredAttachment != null)
                    {
                        _context.Attachments.Remove(coverage.AdditionalInsuredAttachment);
                        coverage.AdditionalInsuredAttachment = null;
                    }
                    break;
                case "gl-wos":
                    var coveragewos = await _context.GeneralLiabilityCoverages
                    .Include(c => c.WaiverOfSubAttachment)
                    .FirstOrDefaultAsync(c => c.GeneralLiabilityCoverageId == coverageId);

                    if (coveragewos == null) return;

                    if (coveragewos.WaiverOfSubAttachment != null)
                    {
                        _context.Attachments.Remove(coveragewos.WaiverOfSubAttachment);
                        coveragewos.WaiverOfSubAttachment = null;
                    }
                    break;
            }

            await _context.SaveChangesAsync();
        }

        //Attachments
        public async Task SaveDropZoneAttachmentAsync(Attachment attachment)
        {
            await _log.LogAsync(LogLevel.Information, $"Saving attachment: {attachment.OriginalFileName}", "AttachmentService");
            // Get the current user
            var userId = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            attachment.UploadedById = userId;
            attachment.DateCreated = DateTime.UtcNow;

            // Paths
            var tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "temp");
            var tempFilePath = Path.Combine(tempFolder, attachment.OriginalFileName); // fileName is already set on attachment

            // Determine target storage path segments
            var absolutePathSegments = new List<string> { Directory.GetCurrentDirectory(), "wwwroot", "uploads" };
            var relativePathSegments = new List<string> { "uploads" };

            void AppendSegments(params string[] segments)
            {
                absolutePathSegments.AddRange(segments);
                relativePathSegments.AddRange(segments);
            }

            if (attachment.ClientId.HasValue && attachment.AttachmentGroupId.HasValue)
            {
                AppendSegments("clients", attachment.ClientId.Value.ToString(), attachment.AttachmentGroupId.Value.ToString());
            }
            else if ((attachment.IsInvoice || attachment.IsFinanceAgreement) && attachment.ClientId.HasValue)
            {
                AppendSegments("clients", attachment.ClientId.Value.ToString());
                if (attachment.AttachmentGroupId.HasValue)
                {
                    AppendSegments(attachment.AttachmentGroupId.Value.ToString());
                }
                else if (attachment.RenewalId.HasValue)
                {
                    AppendSegments(attachment.RenewalId.Value.ToString());
                }
                else
                {
                    AppendSegments(attachment.ClientId.Value.ToString());
                }
            }
            else if (attachment.PolicyId.HasValue)
            {
                AppendSegments("policies", attachment.PolicyId.Value.ToString());
            }
            else if (attachment.ClientId.HasValue)
            {
                AppendSegments("clients", attachment.ClientId.Value.ToString());
            }
            else if (attachment.CarrierId.HasValue)
            {
                AppendSegments("carriers", attachment.CarrierId.Value.ToString());
            }
            else if (attachment.RenewalId.HasValue)
            {
                AppendSegments("renewals", attachment.RenewalId.Value.ToString());
            }
            else if (attachment.AttachmentGroupId.HasValue)
            {
                AppendSegments("attachments", attachment.AttachmentGroupId.Value.ToString());
            }

            if (absolutePathSegments.Count > 3)
            {
                var uploadsFolder = Path.Combine(absolutePathSegments.ToArray());
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                // Get the original filename && extension (from the original file name, not FileFormat)
                var originalFileName = Path.GetFileNameWithoutExtension(attachment.OriginalFileName);
                var fileExtension = Path.GetExtension(attachment.OriginalFileName);

                // Generate a five-character hash
                var hash = StringHelper.GenerateFiveCharacterHash(originalFileName);
                
                // Create a new filename with the hash appended
                attachment.HashedFileName = $"{originalFileName}_{hash}{fileExtension}";

                var normalizedExtension = fileExtension?.ToLowerInvariant() ?? string.Empty;
                var relativeLocalPath = Path.Combine(relativePathSegments.ToArray());
                var targetFolder = uploadsFolder;

                var isDataFile = normalizedExtension == ".json" || normalizedExtension == ".txt";
                if (isDataFile)
                {
                    targetFolder = Path.Combine(uploadsFolder, ".data");
                    if (!Directory.Exists(targetFolder))
                    {
                        Directory.CreateDirectory(targetFolder);
                    }
                    relativeLocalPath = Path.Combine(relativeLocalPath, ".data");
                }

                var newFilePath = Path.Combine(targetFolder, attachment.HashedFileName);
                
                // Wait until temp file is fully ready (not changing size && can be opened exclusively)
                await WaitForTempFileReady(tempFilePath);

                // Retry mechanism for file move operation to handle file locking issues
                bool moveSuccess = false;
                int maxRetries = 5;
                int retryDelay = 500; // milliseconds
                
                for (int attempt = 1; attempt <= maxRetries; attempt++)
                {
                    try
                    {
                        // Add a small delay to ensure file handle is released
                        if (attempt > 1)
                        {
                            await Task.Delay(retryDelay * attempt);
                        }
                        
                        System.IO.File.Move(tempFilePath, newFilePath);
                        Console.WriteLine($"Moved {tempFilePath} to {newFilePath}");
                        moveSuccess = true;
                        break;
                    }
                    catch (IOException ex) when (attempt < maxRetries)
                    {
                        Console.WriteLine($"Attempt {attempt} failed to move file: {ex.Message}. Retrying...");
                        await _log.LogAsync(LogLevel.Warning, $"File move attempt {attempt} failed: {ex.Message}. Retrying...", "AttachmentService");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to move file: {ex}");
                        await _log.LogAsync(LogLevel.Error, $"Failed to move file: {ex.Message}", "AttachmentService", ex);
                        throw;
                    }
                }
                
                if (!moveSuccess)
                {
                    var errorMessage = $"Failed to move file after {maxRetries} attempts due to file locking issues.";
                    Console.WriteLine(errorMessage);
                    await _log.LogAsync(LogLevel.Error, errorMessage, "AttachmentService");
                    throw new IOException(errorMessage);
                }

                attachment.LocalPath = relativeLocalPath.Replace("\\", "/");

                //Generate thumbnail if the file is a PDF
                if (Path.GetExtension(attachment.HashedFileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    var thumbnailFolder = Path.Combine(uploadsFolder, ".data");
                    if (!Directory.Exists(thumbnailFolder))
                    {
                        Directory.CreateDirectory(thumbnailFolder);
                    }
                    var thumbnailFilePath = Path.Combine(thumbnailFolder, Path.GetFileNameWithoutExtension(attachment.HashedFileName) + "_thumb.jpg");
                    try
                    {
                        GeneratePdfThumbnail(newFilePath, thumbnailFilePath);
                    }
                    catch (Exception ex)
                    {
                        // If thumbnail generation fails, log && continue without crashing the circuit
                        Console.WriteLine($"Thumbnail generation failed: {ex.Message}");
                        await _log.LogAsync(LogLevel.Warning, $"Thumbnail generation failed for {attachment.HashedFileName}: {ex.Message}", "AttachmentService", ex);
                    }
                }
            }
            else
            {
                // Handle error: No associated entity
                await _log.LogAsync(LogLevel.Error, "Attachment not associated with a entity", "AttachmentService");
                throw new Exception("Attachment must be associated with an entity.");
                
            }

            // Save attachment to database
            using var context = _contextFactory.CreateDbContext();
            context.Attachments.Add(attachment);
            await context.SaveChangesAsync();
        }

        private static async Task WaitForTempFileReady(string tempFilePath, int maxWaitMs = 8000, int checkIntervalMs = 150)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            long lastSize = -1;
            while (sw.ElapsedMilliseconds < maxWaitMs)
            {
                try
                {
                    if (System.IO.File.Exists(tempFilePath))
                    {
                        using (var fs = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
                        {
                            var size = fs.Length;
                            if (size > 0)
                            {
                                if (lastSize == size)
                                {
                                    // Size stable across checks && can be opened exclusively
                                    return;
                                }
                                lastSize = size;
                            }
                        }
                    }
                }
                catch
                {
                    // File is still being written || locked; wait && retry
                }
                await Task.Delay(checkIntervalMs);
            }
            // If we exit the loop, proceed anyway; move retry logic may still succeed
        }

        public async Task<List<Attachment>> GetAttachmentsByClientIdAsync(int clientId)
        {
            using var context = _contextFactory.CreateDbContext();
            var myrecords = await context.Attachments
                .Include(a => a.Folder)
                .Include(a => a.Policy)
                .Include(a => a.UploadedBy)
                .Where(a => a.ClientId == clientId)
                .ToListAsync();

            return myrecords;
        }

        public async Task<List<Folder>> GetFoldersAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Folders.ToListAsync();
        }

        public async Task<Attachment> GetAttachmentByIdAsync(int attachmentId)
        {
            using var context = _contextFactory.CreateDbContext();
            var attachment = await context.Attachments
                .Include(a => a.Folder)
                .Include(a => a.Policy)
                    .ThenInclude(p => p.Carrier)
                .Include(a => a.Policy)
                    .ThenInclude(p => p.Wholesaler)
                .Include(a => a.Policy)
                    .ThenInclude(p => p.Product)
                .Include(a => a.UploadedBy)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AttachmentId == attachmentId);
            if (attachment != null)
            {
                return attachment;
            }
            else { return null; }
        }

        //Renewal && Invoices
        public async Task<List<Attachment>> GetAttachmentsByPolicyIdAsync(int policyId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Attachments
                .Where(a => a.PolicyId == policyId)
                .OrderByDescending(a => a.DateCreated)
                .ToListAsync();
        }

        /// <summary>
        /// Deletes the attachment record from the database && deletes the file from the filesystem.
        /// </summary>
        public async Task DeleteAttachmentAndFileAsync(int attachmentId)
        {
            using var context = _contextFactory.CreateDbContext();
            var attachment = await context.Attachments.FirstOrDefaultAsync(a => a.AttachmentId == attachmentId);
            if (attachment == null)
                return;
            // Build the file path
            try
            {
                if (!string.IsNullOrEmpty(attachment.LocalPath) && !string.IsNullOrEmpty(attachment.HashedFileName))
                {
                    var root = Directory.GetCurrentDirectory();
                    var filePath = Path.Combine(root, "wwwroot", attachment.LocalPath.Replace("/", Path.DirectorySeparatorChar.ToString()), attachment.HashedFileName);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    // Also try to delete the thumbnail if it exists
                    var normalizedLocalPath = attachment.LocalPath.Replace("\\", "/");
                    var thumbFileName = Path.GetFileNameWithoutExtension(attachment.HashedFileName) + "_thumb.jpg";
                    var baseFolder = Path.Combine(root, "wwwroot", normalizedLocalPath.Replace("/", Path.DirectorySeparatorChar.ToString()));

                    string dataThumbPath;
                    if (normalizedLocalPath.EndsWith("/.data", StringComparison.OrdinalIgnoreCase))
                    {
                        dataThumbPath = Path.Combine(baseFolder, thumbFileName);
                    }
                    else
                    {
                        dataThumbPath = Path.Combine(baseFolder, ".data", thumbFileName);
                    }

                    if (File.Exists(dataThumbPath))
                    {
                        File.Delete(dataThumbPath);
                    }
                    else
                    {
                        var legacyThumbPath = Path.Combine(baseFolder, thumbFileName);
                        if (File.Exists(legacyThumbPath))
                        {
                            File.Delete(legacyThumbPath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await _log.LogAsync(LogLevel.Error, $"Error deleting attachment file: {ex.Message}", "AttachmentService", ex);
            }
            context.Attachments.Remove(attachment);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Saves an attachment directly to the database when the file is already in its final location.
        /// This bypasses the temp file process used by SaveDropZoneAttachmentAsync.
        /// </summary>
        public async Task SaveAttachmentDirectlyAsync(Attachment attachment)
        {
            await _log.LogAsync(LogLevel.Information, $"Saving attachment directly: {attachment.OriginalFileName}", "AttachmentService");
            
            // Get the current user
            var currentUser = _stateService.CurrentUser;
            attachment.UploadedById = currentUser.Id;
            attachment.DateCreated = DateTime.UtcNow;

            // Save attachment to database
            using var context = _contextFactory.CreateDbContext();
            context.Attachments.Add(attachment);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAttachmentAsync(int attachmentId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            
            // Get the attachment with all its navigation properties
            var attachment = await context.Attachments
                .Include(a => a.Renewal)
                .Include(a => a.AttachmentGroup)
                .Include(a => a.Policy)
                .Include(a => a.Carrier)
                .Include(a => a.Submission)
                .FirstOrDefaultAsync(a => a.AttachmentId == attachmentId);

            if (attachment == null)
            {
                throw new ArgumentException($"Attachment with ID {attachmentId} not found.");
            }

            // Get the server file path using StringHelper
            var serverFilePath = StringHelper.BuildWindowsPath(attachment, false, false, true);

            // Delete the physical file if it exists
            if (File.Exists(serverFilePath))
            {
                try
                {
                    File.Delete(serverFilePath);
                }
                catch (Exception ex)
                {
                    // Log the error but continue with database deletion
                    Console.WriteLine($"Error deleting physical file: {ex.Message}");
                }
            }

            // Remove from database
            context.Attachments.Remove(attachment);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes the attachment record from the database only.
        /// </summary>
        public async Task DeleteAttachmentAsync2(int attachmentId)
        {
            using var context = _contextFactory.CreateDbContext();
            var attachment = await context.Attachments.FirstOrDefaultAsync(a => a.AttachmentId == attachmentId);
            if (attachment == null)
                return;
            context.Attachments.Remove(attachment);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the Description (display name) of an attachment.
        /// </summary>
        public async Task UpdateAttachmentDescriptionAsync(int attachmentId, string newDescription)
        {
            if (string.IsNullOrWhiteSpace(newDescription)) return;
            using var context = _contextFactory.CreateDbContext();
            var attachment = await context.Attachments.FirstOrDefaultAsync(a => a.AttachmentId == attachmentId);
            if (attachment == null) return;
            attachment.Description = newDescription.Trim();
            await context.SaveChangesAsync();
        }

        // Generates a thumbnail for the first page of a PDF file
        private void GeneratePdfThumbnail(string pdfPath, string thumbnailFilePath)
        {
            // Initialize PDF to Image converter && convert the first page to an image
            using (var inputStream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read))
            {
                PdfToImageConverter imageConverter = new PdfToImageConverter();
                imageConverter.Load(inputStream);
                // Convert the first page to an image
                using (var outputStream = imageConverter.Convert(0, false, false))
                {
                    using (var memoryStream = outputStream as MemoryStream)
                    {
                        var imageBytes = memoryStream.ToArray();
                        using (var image = (Bitmap)System.Drawing.Image.FromStream(new MemoryStream(imageBytes)))
                        {
                            // Resize the image to a 300px width, keeping the aspect ratio
                            int thumbnailWidth = 300;
                            int thumbnailHeight = (int)(image.Height * (300.0 / image.Width));

                            using (var thumbnail = new Bitmap(image, new System.Drawing.Size(thumbnailWidth, thumbnailHeight)))
                            {
                                thumbnail.Save(thumbnailFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                                Console.WriteLine("Thumbnail saved!");
                            }
                        }
                    }
                }
            }
        }
    }
}