using Mantis.Data;
using Mantis.Domain.Policies.Models;
using Mantis.Domain.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Mantis.Domain.Shared.Services
{
    public class AttachmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AttachmentService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Attachment> AddPolicyAttachmentAsync(string fileName, int coverageId, string attachmentType)
        {
            switch (attachmentType.ToLower())
            {
                case "ai": // Additional Insured
                    var attachment = new Attachment
                    {
                        FileName = fileName,
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
                        FileName = fileName,
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
                        FileName = fileName,
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

    }
}
