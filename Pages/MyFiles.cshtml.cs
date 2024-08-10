using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using metalduck.Data;
using metalduck.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace metalduck.Pages;

public class MyFilesModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MyFilesModel> _logger;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public MyFilesModel(ApplicationDbContext context, ILogger<MyFilesModel> logger, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public bool IsSignedIn = false;
    public IList<Document> Documents { get; set; } = new List<Document>();
    [BindProperty(SupportsGet = true)]
    public string Search { get; set; }

    public void OnGet()
    {
        string userId = _userManager.GetUserId(User)!;
        IsSignedIn = _signInManager.IsSignedIn(User);

        Documents = _context.Documents
            .Where(d => d.OwnerId == userId && d.Status != DocumentStatus.Rejected && EF.Functions.Like(d.FileName, $"%{Search}%"))
            .Include(d => d.Owner)
            .ToList();
    }
}

