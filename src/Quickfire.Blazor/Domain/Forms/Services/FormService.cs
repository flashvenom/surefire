using Quickfire.Blazor.Data;
using Quickfire.Blazor.Domain.Forms.Models;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Pdf.Parsing;
using Quickfire.Blazor.Domain.Shared.Services;
using Quickfire.Blazor.Domain.Shared.Helpers;
using Quickfire.Blazor.Domain.Attachments.Models;
using Quickfire.Blazor.Domain.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.IO;

namespace Quickfire.Blazor.Domain.Forms.Services
{
    public partial class FormService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly StateService _stateService;
        private readonly ILogger<FormService> _logger;

        public FormService(
            IDbContextFactory<ApplicationDbContext> dbContextFactory,
            StateService stateService,
            ILogger<FormService> logger)
        {
            _dbContextFactory = dbContextFactory;
            _stateService = stateService;
            _logger = logger;
        }

        // [GET] ----------------------------------------------------------------------//
        public async Task<List<FormPdf>> GetAllFormPdfs()
        {
            using var context = _dbContextFactory.CreateDbContext();
            //Order by alphabetically
            var formpdflist = await context.FormPdf.OrderBy(fp => fp.Title).ToListAsync();
            return formpdflist;
        }
        public async Task<Certificate> GetCertificateByIdAsync(int certid)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var certificate = await context.Certificates.FirstOrDefaultAsync(p => p.CertificateId == certid);

            return certificate;
        }
        public async Task<FormDoc> GetFormDocByIdAsync(int formDocId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var formDoc = await context.FormDocs
                .Include(fd => fd.Client)      // Include the Client entity
                .Include(fd => fd.Lead)      // Include the Client entity
                .Include(fd => fd.CreatedBy)   // Include the CreatedBy entity
                .Include(fd => fd.ModifiedBy)  // Include the ModifiedBy entity
                .Include(fd => fd.FormPdf)     // Include the FormPdf entity
                .Include(fd => fd.Submission)  // Include Submission
                .Include(fd => fd.Policy)      // Include Policy
                .Include(fd => fd.Renewal)     // Include Renewal
                .FirstOrDefaultAsync(p => p.FormDocId == formDocId);

            return formDoc;
        }

        // New method to get form docs by entity type and ID
        public async Task<List<FormDoc>> GetFormDocsByEntityAsync(string entityType, int entityId, int page = 1, int pageSize = 10, string sortField = "DateModified", bool sortAscending = false)
        {
            using var context = _dbContextFactory.CreateDbContext();
            IQueryable<FormDoc> query = context.FormDocs
                .Include(fd => fd.FormPdf)
                .Include(fd => fd.CreatedBy)
                .Include(fd => fd.ModifiedBy);

            // Filter by entity type and ID
            switch (entityType.ToLower())
            {
                case "client":
                    query = query.Where(fd => fd.ClientId == entityId);
                    break;
                case "lead":
                    query = query.Where(fd => fd.LeadId == entityId);
                    break;
                case "submission":
                    query = query.Where(fd => fd.SubmissionId == entityId);
                    break;
                case "policy":
                    query = query.Where(fd => fd.PolicyId == entityId);
                    break;
                case "renewal":
                    query = query.Where(fd => fd.RenewalId == entityId);
                    break;
                default:
                    throw new ArgumentException("Invalid entity type", nameof(entityType));
            }

            // Apply sorting
            query = ApplySorting(query, sortField, sortAscending);

            // Apply paging
            var skip = (page - 1) * pageSize;
            return await query.Skip(skip).Take(pageSize).ToListAsync();
        }

        // Helper method to apply sorting
        private IQueryable<FormDoc> ApplySorting(IQueryable<FormDoc> query, string sortField, bool sortAscending)
        {
            switch (sortField.ToLower())
            {
                case "title":
                    return sortAscending ? query.OrderBy(fd => fd.Title) : query.OrderByDescending(fd => fd.Title);
                case "datecreated":
                    return sortAscending ? query.OrderBy(fd => fd.DateCreated) : query.OrderByDescending(fd => fd.DateCreated);
                case "datemodified":
                    return sortAscending ? query.OrderBy(fd => fd.DateModified) : query.OrderByDescending(fd => fd.DateModified);
                case "createdby":
                    return sortAscending ? query.OrderBy(fd => fd.CreatedBy.FirstName) : query.OrderByDescending(fd => fd.CreatedBy.FirstName);
                default:
                    return query.OrderByDescending(fd => fd.DateModified);
            }
        }

        // Get count of form docs for pagination
        public async Task<int> GetFormDocsCountByEntityAsync(string entityType, int entityId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            IQueryable<FormDoc> query = context.FormDocs;

            // Filter by entity type and ID
            switch (entityType.ToLower())
            {
                case "client":
                    query = query.Where(fd => fd.ClientId == entityId);
                    break;
                case "lead":
                    query = query.Where(fd => fd.LeadId == entityId);
                    break;
                case "submission":
                    query = query.Where(fd => fd.SubmissionId == entityId);
                    break;
                case "policy":
                    query = query.Where(fd => fd.PolicyId == entityId);
                    break;
                case "renewal":
                    query = query.Where(fd => fd.RenewalId == entityId);
                    break;
                default:
                    throw new ArgumentException("Invalid entity type", nameof(entityType));
            }

            return await query.CountAsync();
        }

        // [REVISIONS] ---------------------------------------------------------------//
        // Get all revisions for a form doc
        public async Task<List<FormDocRevision>> GetFormDocRevisionsAsync(int formDocId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.FormDocRevisions
                .Include(fdr => fdr.CreatedBy)
                .Where(fdr => fdr.FormDocId == formDocId)
                .OrderByDescending(fdr => fdr.DateCreated)
                .ToListAsync();
        }

        // Get a specific revision
        public async Task<FormDocRevision> GetFormDocRevisionByIdAsync(int revisionId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.FormDocRevisions
                .Include(fdr => fdr.CreatedBy)
                .FirstOrDefaultAsync(fdr => fdr.FormDocRevisionId == revisionId);
        }

        // Create a new revision
        public async Task<int> CreateFormDocRevisionAsync(int formDocId, string jsonData, string revisionName = null)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var currentUser = _stateService.CurrentUser;
            context.Attach(currentUser);

            var formDoc = await context.FormDocs.FindAsync(formDocId);
            if (formDoc == null)
            {
                throw new Exception("FormDoc not found.");
            }

            // If no revision name provided, create a default one
            if (string.IsNullOrEmpty(revisionName))
            {
                var revisionCount = await context.FormDocRevisions
                    .Where(fdr => fdr.FormDocId == formDocId)
                    .CountAsync();
                revisionName = $"Revision {revisionCount + 1}";
            }

            var revision = new FormDocRevision
            {
                FormDocId = formDocId,
                RevisionName = revisionName,
                JSONData = jsonData,
                CreatedBy = currentUser,
                DateCreated = DateTime.UtcNow
            };

            context.FormDocRevisions.Add(revision);
            await context.SaveChangesAsync();
            return revision.FormDocRevisionId;
        }

        // Restore a revision (make it the current version)
        public async Task RestoreFormDocRevisionAsync(int formDocId, int revisionId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var currentUser = _stateService.CurrentUser;
            context.Attach(currentUser);

            var formDoc = await context.FormDocs.FindAsync(formDocId);
            var revision = await context.FormDocRevisions.FindAsync(revisionId);

            if (formDoc == null || revision == null || revision.FormDocId != formDocId)
            {
                throw new Exception("FormDoc or revision not found or doesn't match.");
            }

            // Create a new revision with the current data before updating
            await CreateFormDocRevisionAsync(formDocId, formDoc.JSONData, "Backup before restore");

            // Update the form doc with the revision data
            formDoc.JSONData = revision.JSONData;
            formDoc.ModifiedBy = currentUser;
            formDoc.DateModified = DateTime.UtcNow;

            context.FormDocs.Update(formDoc);
            await context.SaveChangesAsync();
        }

        // [CREATE / DUPE] -----------------------------------------------------------//
        public async Task<int> CreateCertificate(int clientid)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var currentUser = _stateService.CurrentUser;
            context.Attach(currentUser);

            var newcert = new Certificate
            {
                ClientId = clientid,
                HolderName = "New Certificate",
                JSONData = "{}",
                CreatedBy = currentUser,
                ModifiedBy = currentUser,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow
            };
            context.Certificates.Add(newcert);
            await context.SaveChangesAsync();

            return newcert.CertificateId;
        }

        /// <summary>
        /// Gets an existing FormDoc for a policy or creates one based on ProductId mapping
        /// </summary>
        /// <param name="policyId">The policy ID</param>
        /// <returns>The FormDoc for the policy</returns>
        public async Task<FormDoc> GetOrCreateFormDocForPolicyAsync(int policyId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            
            // First, try to get existing FormDoc for this policy
            var existingFormDoc = await context.FormDocs
                .Include(fd => fd.FormPdf)
                .Include(fd => fd.Policy)
                    .ThenInclude(p => p.Product)
                .Include(fd => fd.CreatedBy)
                .Include(fd => fd.ModifiedBy)
                .FirstOrDefaultAsync(fd => fd.PolicyId == policyId);

            if (existingFormDoc != null)
            {
                return existingFormDoc;
            }

            // If no existing FormDoc, create one based on ProductId mapping
            var policy = await context.Policies
                .Include(p => p.Product)
                .FirstOrDefaultAsync(p => p.PolicyId == policyId);

            if (policy == null)
            {
                throw new Exception($"Policy with ID {policyId} not found.");
            }

            // Determine FormPdfId based on ProductId
            int formPdfId = await GetFormPdfIdForProduct(policy.ProductId);

            // Create the FormDoc
            var currentUser = _stateService.CurrentUser;
            context.Attach(currentUser);

            var formPdf = await context.FormPdf.FindAsync(formPdfId);
            if (formPdf == null)
            {
                throw new Exception($"FormPdf with ID {formPdfId} not found.");
            }

            var newFormDoc = new FormDoc
            {
                Title = $"{policy.Product?.LineName ?? "Policy"} Application",
                Description = $"Application form for policy {policy.PolicyNumber}",
                JSONData = "{}",
                FormPdf = formPdf,
                PolicyId = policyId,
                ClientId = policy.ClientId,
                CreatedBy = currentUser,
                ModifiedBy = currentUser,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow
            };

            context.FormDocs.Add(newFormDoc);
            await context.SaveChangesAsync();

            // Create initial revision
            await CreateFormDocRevisionAsync(newFormDoc.FormDocId, "{}", "Initial Version");

            // Load the complete FormDoc with all navigation properties
            return await context.FormDocs
                .Include(fd => fd.FormPdf)
                .Include(fd => fd.Policy)
                    .ThenInclude(p => p.Product)
                .Include(fd => fd.CreatedBy)
                .Include(fd => fd.ModifiedBy)
                .FirstOrDefaultAsync(fd => fd.FormDocId == newFormDoc.FormDocId);
        }

        /// <summary>
        /// Determines the FormPdfId based on ProductId by querying the FormPdf table
        /// </summary>
        /// <param name="productId">The product ID</param>
        /// <returns>The appropriate FormPdfId</returns>
        private async Task<int> GetFormPdfIdForProduct(int? productId)
        {
            using var context = _dbContextFactory.CreateDbContext();

            if (productId != null)
            {
                // Find the FormPdf where ProductIdLink matches the productId
                var mappedFormPdf = await context.FormPdf
                    .FirstOrDefaultAsync(fp => fp.ProductIdLink == productId.ToString());

                if (mappedFormPdf != null)
                {
                    return mappedFormPdf.FormPdfId;
                }
            }

            // Default to Acord 125 for policy details when no mapping exists
            var defaultFormPdf = await context.FormPdf
                .FirstOrDefaultAsync(fp => fp.Title == "Acord 125 (2016/03)");

            defaultFormPdf ??= await context.FormPdf
                .FirstOrDefaultAsync(fp => fp.Filepath == "a125-2016-03.pdf");

            if (defaultFormPdf != null)
            {
                return defaultFormPdf.FormPdfId;
            }

            var fallbackFormPdf = await context.FormPdf.OrderBy(fp => fp.Title).FirstOrDefaultAsync();
            if (fallbackFormPdf != null)
            {
                return fallbackFormPdf.FormPdfId;
            }

            throw new Exception("No FormPdf entries are available.");
        }

        public async Task<int> CreateFormDoc(int formPdfId, int? clientId = null, int? leadId = null, int? submissionId = null, int? policyId = null, int? renewalId = null)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var currentUser = _stateService.CurrentUser;
            context.Attach(currentUser);

            var formPdf = await context.FormPdf.FindAsync(formPdfId);
            if (formPdf == null)
            {
                throw new Exception("FormPdf not found.");
            }

            // Ensure at least one of the entity IDs is provided
            if (clientId == null && leadId == null && submissionId == null && policyId == null && renewalId == null)
            {
                throw new Exception("At least one entity ID (client, lead, submission, policy, or renewal) must be provided.");
            }

            // Create new FormDoc
            var newformdoc = new FormDoc
            {
                Title = "New " + formPdf.Title,
                Description = formPdf.Description,
                JSONData = formPdf.JSONFields,
                FormPdf = formPdf,
                CreatedBy = currentUser,
                ModifiedBy = currentUser,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow
            };

            // Set entity IDs based on the provided parameters
            if (clientId != null) newformdoc.ClientId = clientId.Value;
            if (leadId != null) newformdoc.LeadId = leadId.Value;
            if (submissionId != null) newformdoc.SubmissionId = submissionId.Value;
            if (policyId != null) newformdoc.PolicyId = policyId.Value;
            if (renewalId != null) newformdoc.RenewalId = renewalId.Value;

            context.FormDocs.Add(newformdoc);
            await context.SaveChangesAsync();

            // Create initial revision
            await CreateFormDocRevisionAsync(newformdoc.FormDocId, "{}", "Initial Version");

            return newformdoc.FormDocId;
        }
        public async Task<int> DuplicateCertificateAsync(Certificate originalCertificate)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var currentUser = _stateService.CurrentUser;
            context.Attach(currentUser);

            var duplicatedCertificate = new Certificate
            {
                ClientId = originalCertificate.ClientId,
                HolderName = originalCertificate.HolderName,
                ProjectName = originalCertificate.ProjectName,
                JSONData = originalCertificate.JSONData,
                AttachGLAI = originalCertificate.AttachGLAI,
                AttachGLAIfilename = originalCertificate.AttachGLAIfilename,
                AttachGLWOS = originalCertificate.AttachGLWOS,
                AttachGLWOSfilename = originalCertificate.AttachGLWOSfilename,
                AttachWCWOS = originalCertificate.AttachWCWOS,
                AttachWCWOSfilename = originalCertificate.AttachWCWOSfilename,
                BlockAttachments = originalCertificate.BlockAttachments
            };
            duplicatedCertificate.CreatedBy = currentUser;
            duplicatedCertificate.ModifiedBy = currentUser;
            duplicatedCertificate.DateCreated = DateTime.UtcNow;
            duplicatedCertificate.DateModified = DateTime.UtcNow;

            context.Certificates.Add(duplicatedCertificate);
            await context.SaveChangesAsync();

            return duplicatedCertificate.CertificateId;
        }
        public async Task<int> DuplicateFormDocAsync(FormDoc originalFormdoc)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var currentUser = _stateService.CurrentUser;
            context.Attach(currentUser);

            var newFormdoc = new FormDoc
            {
                ClientId = originalFormdoc.ClientId,
                LeadId = originalFormdoc.LeadId,
                SubmissionId = originalFormdoc.SubmissionId,
                PolicyId = originalFormdoc.PolicyId,
                RenewalId = originalFormdoc.RenewalId,
                Title = originalFormdoc.Title + " (Copy)",
                Description = originalFormdoc.Description,
                JSONData = originalFormdoc.JSONData,
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now,
                FormPdf = originalFormdoc.FormPdf
            };
            newFormdoc.CreatedBy = currentUser;
            newFormdoc.ModifiedBy = currentUser;
            newFormdoc.DateCreated = DateTime.UtcNow;
            newFormdoc.DateModified = DateTime.UtcNow;

            context.FormDocs.Add(newFormdoc);
            await context.SaveChangesAsync();

            // Create initial revision for the duplicate
            await CreateFormDocRevisionAsync(newFormdoc.FormDocId, newFormdoc.JSONData, "Initial Version (Duplicated)");

            return newFormdoc.FormDocId;
        }

        // [UPDATE] ------------------------------------------------------------------//
        public async Task UpdateCertificate(Certificate certificate)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var currentUser = _stateService.CurrentUser;
            context.Attach(currentUser);

            var existingCertificate = await context.Certificates.FindAsync(certificate.CertificateId);
            if (existingCertificate != null)
            {
                existingCertificate.HolderName = certificate.HolderName;
                existingCertificate.JSONData = certificate.JSONData;
                existingCertificate.ModifiedBy = currentUser;
                existingCertificate.DateModified = DateTime.UtcNow;
                context.Certificates.Update(existingCertificate);
                await context.SaveChangesAsync();
            }
        }
        public async Task UpdateFormDoc(FormDoc formdoc)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var currentUser = _stateService.CurrentUser;
            context.Attach(currentUser);

            var existingFormDoc = await context.FormDocs.FindAsync(formdoc.FormDocId);
            if (existingFormDoc != null)
            {
                // Create a revision with the old data before updating
                await CreateFormDocRevisionAsync(existingFormDoc.FormDocId, existingFormDoc.JSONData);

                // Update the form doc
                existingFormDoc.Title = formdoc.Title;
                existingFormDoc.Description = formdoc.Description;
                existingFormDoc.JSONData = formdoc.JSONData;
                existingFormDoc.ModifiedBy = currentUser;
                existingFormDoc.DateModified = DateTime.UtcNow;

                context.FormDocs.Update(existingFormDoc);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateFormDocWithRevisionCheck(FormDoc formdoc)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var currentUser = _stateService.CurrentUser;
            context.Attach(currentUser);

            var existingFormDoc = await context.FormDocs.FindAsync(formdoc.FormDocId);
            if (existingFormDoc != null)
            {
                // Only create a revision if the JSON data has actually changed
                if (existingFormDoc.JSONData != formdoc.JSONData)
                {
                    await CreateFormDocRevisionAsync(existingFormDoc.FormDocId, existingFormDoc.JSONData);
                }

                // Update the form doc
                existingFormDoc.Title = formdoc.Title;
                existingFormDoc.Description = formdoc.Description;
                existingFormDoc.JSONData = formdoc.JSONData;
                existingFormDoc.ModifiedBy = currentUser;
                existingFormDoc.DateModified = DateTime.UtcNow;

                context.FormDocs.Update(existingFormDoc);
                await context.SaveChangesAsync();
            }
        }

        // TOOLS ---------------------------------------------------------------------//
        public byte[] FlattenPdf(byte[] pdfBytes)
        {
            using (MemoryStream stream = new MemoryStream(pdfBytes))
            {
                PdfLoadedDocument loadedDocument = new PdfLoadedDocument(stream);
                PdfLoadedForm loadedForm = loadedDocument.Form;
                loadedForm.Flatten = true;
                using (MemoryStream outputStream = new MemoryStream())
                {
                    loadedDocument.Save(outputStream);
                    loadedDocument.Close(true);
                    return outputStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Updates the status and notes of a certificate request
        /// </summary>
        /// <param name="requestId">The ID of the certificate request to update</param>
        /// <param name="status">The new status</param>
        /// <param name="notes">The updated notes</param>
        /// <returns>True if the update was successful, false otherwise</returns>
        public async Task<bool> UpdateCertificateRequestStatusAsync(int requestId, string status, string notes)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                var request = await context.CertificateRequests.FindAsync(requestId);
                
                if (request == null)
                {
                    return false;
                }
                
                request.Status = status;
                request.Notes = notes;
                
                if (status == "Approved" || status == "Completed")
                {
                    request.ImportedDate = DateTime.UtcNow;
                }
                
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating certificate request status for request {requestId}");
                return false;
            }
        }
        
        /// <summary>
        /// Deletes a certificate request by ID
        /// </summary>
        /// <param name="requestId">The ID of the certificate request to delete</param>
        /// <returns>True if delete successful, false otherwise</returns>
        public async Task<bool> DeleteCertificateRequestAsync(int requestId)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                var request = await context.CertificateRequests.FindAsync(requestId);
                
                if (request == null)
                {
                    return false;
                }
                
                context.CertificateRequests.Remove(request);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting certificate request {requestId}");
                return false;
            }
        }

        /// <summary>
        /// Deletes a form document and all its associated revisions
        /// </summary>
        /// <param name="formDocId">The ID of the form document to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        public async Task<bool> DeleteFormDocAsync(int formDocId)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                
                // First, delete all revisions associated with this form
                var revisions = await context.FormDocRevisions
                    .Where(fdr => fdr.FormDocId == formDocId)
                    .ToListAsync();
                
                context.FormDocRevisions.RemoveRange(revisions);
                
                // Then delete the form document itself
                var formDoc = await context.FormDocs.FindAsync(formDocId);
                if (formDoc == null)
                {
                    _logger.LogWarning($"Form document {formDocId} not found for deletion");
                    return false;
                }
                
                context.FormDocs.Remove(formDoc);
                await context.SaveChangesAsync();
                
                _logger.LogInformation($"Successfully deleted form document {formDocId} and its {revisions.Count} revisions");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting form document {formDocId}");
                throw; // Re-throw the exception to be handled by the UI
            }
        }

        /// <summary>
        /// Gets all certificate requests from the internal store.
        /// </summary>
        public async Task<List<CertificateRequest>> GetAllCertificateRequestsAsync()
        {
            try
            {
                return await GetInternalCertificateRequestsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all certificate requests");
                return new List<CertificateRequest>();
            }
        }
        
        /// <summary>
        /// Gets a certificate request by ID from the internal store.
        /// </summary>
        /// <param name="requestId">The ID of the certificate request</param>
        /// <returns>The certificate request if found, null otherwise</returns>
        public async Task<CertificateRequest?> GetCertificateRequestByIdAsync(int requestId)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                return await context.CertificateRequests.FindAsync(requestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving certificate request {requestId}");
                return null;
            }
        }
    }
}
