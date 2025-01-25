using System.Drawing;
using Surefire.Data;
using Surefire.Domain.Attachments.Models;
using Surefire.Domain.Shared.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Surefire.Domain.Logs;

namespace Surefire.Domain.Attachments.Services
{
    public class AttachmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly ILoggingService _log;

        public AttachmentService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, ILoggingService log, IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _contextFactory = contextFactory;
            _log = log;
        }

        //Headshots and Logos and Such
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
        public async Task RemoveHeadshotFromContact(int contactId)
        {
            // Retrieve the contact by ID
            var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.ContactId == contactId);

            if (contact == null || string.IsNullOrEmpty(contact.HeadshotFilename))
            {
                return; // No contact found or no headshot to remove
            }

            // Construct the file path for the headshot
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/clients", contact.HeadshotFilename);

            // Check if the file exists, and if it does, delete it
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Set the HeadshotFilename to null and update the contact in the database
            contact.HeadshotFilename = null;
            _context.Contacts.Update(contact);
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
            var tempFilePath = Path.Combine(tempFolder, attachment.OriginalFileName);

            // Determine entity folder and ID
            string entityFolder = null;
            string entityId = null;

            if (attachment.ClientId.HasValue)
            {
                entityFolder = "clients";
                entityId = attachment.ClientId.Value.ToString();
            }
            else if (attachment.PolicyId.HasValue)
            {
                entityFolder = "policies";
                entityId = attachment.PolicyId.Value.ToString();
            }
            else if (attachment.CarrierId.HasValue)
            {
                entityFolder = "carriers";
                entityId = attachment.CarrierId.Value.ToString();
            }
            else if (attachment.RenewalId.HasValue)
            {
                entityFolder = "renewals";
                entityId = attachment.RenewalId.Value.ToString();
            }

            if (entityFolder != null && entityId != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", entityFolder, entityId);
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                // Get the original filename and extension
                var originalFileName = Path.GetFileNameWithoutExtension(attachment.OriginalFileName);
                var fileExtension = Path.GetExtension(attachment.FileFormat);

                // Generate a five-character hash
                var hash = StringHelper.GenerateFiveCharacterHash(originalFileName);

                // Create a new filename with the hash appended
                attachment.HashedFileName = $"{originalFileName}_{hash}{fileExtension}";

                var newFilePath = Path.Combine(uploadsFolder, attachment.HashedFileName);
                System.IO.File.Move(tempFilePath, newFilePath);

                attachment.LocalPath = Path.Combine("uploads", entityFolder, entityId);

                //Generate thumbnail if the file is a PDF (Removed 1/24/25 for publishing purposes to reduce install size)
                //if (Path.GetExtension(attachment.FileFormat).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                //{
                //    var thumbnailFilePath = Path.Combine(uploadsFolder, Path.GetFileNameWithoutExtension(attachment.HashedFileName) + "_thumb.jpg");
                //    Console.WriteLine($"Ready: {thumbnailFilePath}");
                //    // Initialize PDF to Image converter and convert the first page to an image
                //    using (var inputStream = new FileStream(newFilePath, FileMode.Open, FileAccess.Read))
                //    {
                //        PdfToImageConverter imageConverter = new PdfToImageConverter();
                //        imageConverter.Load(inputStream);
                //        Console.WriteLine($"PDF Init: {thumbnailFilePath}");
                //        // Convert the first page to an image
                //        using (var outputStream = imageConverter.Convert(0, false, false))
                //        {
                //            Console.WriteLine("1");
                //            using (var memoryStream = outputStream as MemoryStream)
                //            {
                //                Console.WriteLine("2");
                //                var imageBytes = memoryStream.ToArray();
                //                using (var image = (Bitmap)Image.FromStream(new MemoryStream(imageBytes)))
                //                {
                //                    Console.WriteLine("3");
                //                    // Resize the image to a 300px width, keeping the aspect ratio
                //                    int thumbnailWidth = 300;
                //                    int thumbnailHeight = (int)(image.Height * (300.0 / image.Width));

                //                    using (var thumbnail = new Bitmap(image, new Size(thumbnailWidth, thumbnailHeight)))
                //                    {
                //                        Console.WriteLine($"Width: {thumbnailWidth} Height: {thumbnailHeight}");
                //                        thumbnail.Save(thumbnailFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                //                        Console.WriteLine("4");
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
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
        public async Task<List<Attachment>> GetAttachmentsByClientIdAsync(int clientId)
        {
            using var context = _contextFactory.CreateDbContext();
            var myrecords = await context.Attachments
                .Include(a => a.Folder)
                .Include(a => a.Policy)
                .Include(a => a.UploadedBy)
                .Where(a => a.ClientId == clientId)
                .ToListAsync();

            // Debugging
            Console.WriteLine($"Fetched {myrecords.Count} attachments for ClientId: {clientId}");
            foreach (var attachment in myrecords)
            {
                Console.WriteLine($"Attachment ID: {attachment.AttachmentId}, Policy ID: {attachment.Policy?.PolicyId}, Description: {attachment.Description}");
            }

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

        //Renewal and Invoices
        public async Task<List<Attachment>> GetInvoiceAttachmentsByClientAndRenewalIdAsync(int clientId, int renewalId)
        {https://localhost:7074/uploads/clients/63/4550557b-a859-4ea0-8341-4d0f755f9e4a_thumb.jpg
            using var context = _contextFactory.CreateDbContext();

            var attachments = await context.Attachments
                .Include(a => a.Folder) // Include the Folder navigation property
                .Where(a => a.ClientId == clientId && a.RenewalId == renewalId && a.Folder.Name == "Invoice")
                .ToListAsync();

            return attachments;
        }
        public async Task<Attachment> GetMostRecentAttachmentByRenewalIdAsync(int renewalId)
        {
            using var context = _contextFactory.CreateDbContext();

            // Query to find the most recent attachment for the given RenewalId
            var attachment = await context.Attachments
                .Where(a => a.RenewalId == renewalId)
                .OrderByDescending(a => a.DateCreated) // Sort by DateCreated in descending order
                .FirstOrDefaultAsync(); // Take the most recent attachment

            return attachment; // Returns null if no attachment found
        }
    }
}
