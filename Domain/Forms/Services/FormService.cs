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
