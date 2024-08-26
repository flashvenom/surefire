using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Mantis.Data;
using Mantis.Domain.Forms.Models;
using Newtonsoft.Json.Linq;


namespace Mantis.Domain.Shared.Services
{
    [ApiController]
    [Route("api/[controller]")]
    public class CertificateController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CertificateController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpGet("Load/{certificateId}")]
        public async Task<IActionResult> LoadCertificateJson(int certificateId)
        {
            // Load the certificate and related policy data
            var certificate = await _context.Certificates
                .Include(c => c.Client)
                    .ThenInclude(c => c.Policies)
                .FirstOrDefaultAsync(c => c.CertificateId == certificateId);

            if (certificate == null)
            {
                return NotFound("Certificate not found.");
            }

            // Load the existing JSON from the file
            var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/forms/a025-2016-03.json");
            var jsonData = await System.IO.File.ReadAllTextAsync(jsonFilePath);

            // Parse the JSON data and set the Form_CompletionDate field
            var jsonObject = JObject.Parse(jsonData);
            jsonObject["Form_CompletionDate"] = DateTime.UtcNow.ToString("MM/dd/yyyy");

            // Return the updated JSON data
            return Ok(jsonObject.ToString());
        }

    }
}
