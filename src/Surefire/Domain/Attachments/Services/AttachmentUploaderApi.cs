using Microsoft.AspNetCore.Mvc;
using Surefire.Domain.Shared.Helpers;

[ApiController]
[Route("api/[controller]")]
public class AttachmentUploaderController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public AttachmentUploaderController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Save(IFormFile UploadFiles)
    {
        if (UploadFiles.Length > 0)
        {
            // Get the original filename and extension
            var originalFileName = Path.GetFileNameWithoutExtension(UploadFiles.FileName);
            var fileExtension = Path.GetExtension(UploadFiles.FileName);

            // Generate a five-character hash
            var hash = StringHelper.GenerateFiveCharacterHash(originalFileName);

            // Create a new filename with the hash appended
            var hashedFileName = $"{originalFileName}_{hash}{fileExtension}";

            // Temporarily save the file in a temp folder
            var tempFolder = Path.Combine(_environment.WebRootPath, "uploads", "temp");
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            var filePath = Path.Combine(tempFolder, hashedFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await UploadFiles.CopyToAsync(fileStream);
            }

            // Return the hashed filename to the client
            return Ok(new { HashedFileName = hashedFileName });
        }
        return BadRequest();
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SaveOld(IFormFile UploadFiles)
    {
        if (UploadFiles.Length > 0)
        {
            // Generate a hashed filename
            var hashedFileName = Path.GetRandomFileName();

            // Temporarily save the file in a temp folder
            var tempFolder = Path.Combine(_environment.WebRootPath, "uploads", "temp");
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            var filePath = Path.Combine(tempFolder, hashedFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await UploadFiles.CopyToAsync(fileStream);
            }

            // Return the hashed filename to the client
            return Ok(new { HashedFileName = hashedFileName });
        }
        return BadRequest();
    }

    [HttpPost("[action]")]
    public void Remove(string UploadFiles)
    {
        if (!string.IsNullOrEmpty(UploadFiles))
        {
            var tempFolder = Path.Combine(_environment.WebRootPath, "uploads", "temp");
            var filePath = Path.Combine(tempFolder, UploadFiles);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
