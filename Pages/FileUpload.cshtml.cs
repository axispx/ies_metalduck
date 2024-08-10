using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using metalduck.Models;
using metalduck.Data;

namespace metalduck.Pages;

public class FileUploadModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<FileUploadModel> _logger;

    public FileUploadModel(ApplicationDbContext context, UserManager<IdentityUser> userManager, ILogger<FileUploadModel> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IActionResult> OnPost(List<IFormFile> files)
    {
        foreach(var file in files)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
            string timestamp = DateTime.Now.ToFileTime().ToString();
            string extension = Path.GetExtension(file.FileName);

            string fileNameWithTimestamp = fileNameWithoutExtension + "_" + timestamp + extension;

            var filePath = Path.Combine("wwwroot/uploads", fileNameWithTimestamp);

            using(var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);

                var Document = new Document
                {
                    FileName = fileNameWithTimestamp,
                    DateCreated = DateTime.Now,
                    OwnerId = _userManager.GetUserId(User),
                    Status = User.IsInRole("Admin") || User.IsInRole("Manager") ? DocumentStatus.Approved : DocumentStatus.Submitted,
                };

                _context.Documents.Add(Document);
            }
        }

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}

