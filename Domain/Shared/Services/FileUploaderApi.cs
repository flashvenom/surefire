using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;
using Mantis.Data;


namespace Mantis.Domain.Shared.Services
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploaderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FileUploaderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");


        [HttpPost("[action]")]
        public async Task<IActionResult> Save(IFormFile UploadFiles) // Save the uploaded file here
        {
            if (UploadFiles.Length > 0)
            {
                //Create directory if not exists
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                var filePath = Path.Combine(uploads, UploadFiles.FileName);
                if (System.IO.File.Exists(filePath))
                {
                    //Return conflict status code
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    //Save the uploaded file to server
                    await UploadFiles.CopyToAsync(fileStream);
                }
            }
            return Ok();
        }


        [HttpPost("[action]")]
        public void Remove(string UploadFiles) // Delete the uploaded file here
        {
            if (UploadFiles != null)
            {
                var filePath = Path.Combine(uploads, UploadFiles);
                if (System.IO.File.Exists(filePath))
                {
                    //Delete the file from server
                    System.IO.File.Delete(filePath);
                }
            }
        }
    }
}
