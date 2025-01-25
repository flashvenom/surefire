using Surefire.Data;
using Surefire.Domain.Forms.Models;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Pdf.Parsing;
using Surefire.Domain.Shared.Services;

namespace Surefire.Domain.Forms.Services
{
    public class FormService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly StateService _stateService;

        public FormService(IDbContextFactory<ApplicationDbContext> dbContextFactory, StateService stateService)
        {
            _dbContextFactory = dbContextFactory;
            _stateService = stateService;
        }

        // [GET] ----------------------------------------------------------------------//
        public async Task<List<FormPdf>> GetAllFormPdfs()
        {
            using var context = _dbContextFactory.CreateDbContext();
            var formpdflist = await context.FormPdf.ToListAsync();
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
                .FirstOrDefaultAsync(p => p.FormDocId == formDocId);

            return formDoc;
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
        public async Task<int> CreateFormDoc(int formPdfId, int? clientId = null, int? leadId = null)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var currentUser = _stateService.CurrentUser;
            context.Attach(currentUser);

            var formPdf = await context.FormPdf.FindAsync(formPdfId);
            if (formPdf == null)
            {
                throw new Exception("FormPdf not found.");
            }

            // Ensure at least one of clientId or leadId is provided
            if (clientId == null && leadId == null)
            {
                throw new Exception("Either clientId or leadId must be provided.");
            }

            // Create new FormDoc
            var newformdoc = new FormDoc
            {
                Title = "New " + formPdf.Title,
                Description = formPdf.Description,
                JSONData = "{}",
                FormPdf = formPdf,
                CreatedBy = currentUser,
                ModifiedBy = currentUser,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow
            };

            // Set ClientId or LeadId based on the provided parameters
            if (clientId != null)
            {
                newformdoc.ClientId = clientId.Value;
            }

            if (leadId != null)
            {
                newformdoc.LeadId = leadId.Value;
            }

            context.FormDocs.Add(newformdoc);
            await context.SaveChangesAsync();

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
            context.FormDocs.Update(formdoc);
            await context.SaveChangesAsync();
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
    }
}
