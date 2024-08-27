using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Mantis.Data;
using Mantis.Domain.Carriers.Models;
using Mantis.Domain.Policies.Models;
using Mantis.Domain.Attachment.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace Mantis.Domain.Attachment.Services
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

        public async Task AddPolicyAttachment(string fileName, int policyId, string attachTo)
        {
            // Assuming there's a PolicyAttachment model to save the attachments
            var attachment = new Attachment
            {
                FileName = fileName
            };

            _context.Attachments.Add(attachment);
            await _context.SaveChangesAsync();
        }

    }
}
