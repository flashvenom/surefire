using System.Data;
using Microsoft.AspNetCore.Identity;
using Mantis.Data;
using Mantis.Domain.Forms.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;
using System.IO;
using Mantis.Domain.Clients.Models;

namespace Mantis.Domain.Forms.Services
{
    public class FormService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FormService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<List<FormPdf>> GetAllFormPdfs()
        {
            var formpdflist = await _context.FormPdf.ToListAsync();
            return formpdflist;
        }

        public async Task<int> CreateFormDoc(int formPdfId, int clientId)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            var formPdf = await _context.FormPdf.FindAsync(formPdfId);
            if (formPdf == null)
            {
                throw new Exception("FormPdf not found.");
            }

            var newformdoc = new FormDoc
            {
                ClientId = clientId,
                Title = "New " + formPdf.Title,
                Description = formPdf.Description,
                JSONData = "{}",
                FormPdf = formPdf,
                CreatedBy = currentUser,
                ModifiedBy = currentUser,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow
            };
            _context.FormDocs.Add(newformdoc);
            await _context.SaveChangesAsync();

            return newformdoc.FormDocId;
        }

        public async Task<FormDoc> GetFormDocByIdAsync(int formDocId)
        {
            var formDoc = await _context.FormDocs
                .Include(fd => fd.Client)      // Include the Client entity
                .Include(fd => fd.CreatedBy)   // Include the CreatedBy entity
                .Include(fd => fd.ModifiedBy)  // Include the ModifiedBy entity
                .Include(fd => fd.FormPdf)     // Include the FormPdf entity
                .FirstOrDefaultAsync(p => p.FormDocId == formDocId);

            return formDoc;
        }

        public async Task UpdateFormDoc(FormDoc formdoc)
        {
            _context.FormDocs.Update(formdoc);
            await _context.SaveChangesAsync();
        }

        public async Task<int> DuplicateFormDocAsync(FormDoc originalFormdoc)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

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

            _context.FormDocs.Add(newFormdoc);
            await _context.SaveChangesAsync();

            return newFormdoc.FormDocId;
        }



        public async Task<int> CreateCertificate(int clientid)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
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
            _context.Certificates.Add(newcert);
            await _context.SaveChangesAsync();

            return newcert.CertificateId;
        }
        public async Task<Certificate> GetCertificateByIdAsync(int certid)
        {
            var certificate = await _context.Certificates.FirstOrDefaultAsync(p => p.CertificateId == certid);

            return certificate;
        }
        

        
        public async Task UpdateCertificate(Certificate certificate)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var existingCertificate = await _context.Certificates.FindAsync(certificate.CertificateId);
            if (existingCertificate != null)
            {
                existingCertificate.HolderName = certificate.HolderName;
                existingCertificate.JSONData = certificate.JSONData;
                existingCertificate.ModifiedBy = currentUser;
                existingCertificate.DateModified = DateTime.UtcNow;
                _context.Certificates.Update(existingCertificate);
                await _context.SaveChangesAsync();
            }
        }

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

        public async Task StoreTempData(Certificate certificate)
        {
            
            _context.Certificates.Update(certificate);
            await _context.SaveChangesAsync();
            
        }

        public async Task<int> DuplicateCertificateAsync(Certificate originalCertificate)
        {
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            

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

            _context.Certificates.Add(duplicatedCertificate);
            await _context.SaveChangesAsync();

            return duplicatedCertificate.CertificateId;
        }

    }
}
