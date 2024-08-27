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
            var existingCertificate = await _context.Certificates.FindAsync(certificate.CertificateId);
            if (existingCertificate != null)
            {
                existingCertificate.HolderName = certificate.HolderName;
                existingCertificate.JSONData = certificate.JSONData;

                // Update other fields if needed

                _context.Certificates.Update(existingCertificate);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Handle the case where the certificate is not found, if necessary
            }
        }

        public async Task StoreTempData(Certificate certificate)
        {
            
            _context.Certificates.Update(certificate);
            await _context.SaveChangesAsync();
            
        }

        public byte[] FlattenPdf(byte[] pdfBytes)
        {
            using (MemoryStream stream = new MemoryStream(pdfBytes))
            {
                // Load the PDF document
                PdfLoadedDocument loadedDocument = new PdfLoadedDocument(stream);
                PdfLoadedForm loadedForm = loadedDocument.Form;
                PdfLoadedFormFieldCollection fields = loadedForm.Fields;
                // Flatten form fields
                foreach (var item in fields)
                {
                    //item.Flatten = true;
                }
                loadedForm.Flatten = true;
                // Save the flattened document to a memory stream
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
