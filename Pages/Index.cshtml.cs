using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using metalduck.Data;
using metalduck.Models;
using metalduck.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace metalduck.Pages;

[AllowAnonymous]
public class AppIndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AppIndexModel> _logger;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IAuthorizationService _auth;

    public AppIndexModel(ApplicationDbContext context, ILogger<AppIndexModel> logger, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IAuthorizationService auth)
    {
        _context = context;
        _logger = logger;
        _signInManager = signInManager;
        _userManager = userManager;
        _auth = auth;
    }

    public bool IsSignedIn = false;
    public IList<Document> Documents { get; set; } = new List<Document>();
    [BindProperty(SupportsGet = true)]
    public string Search { get; set; }

    public void OnGet()
    {
        string userId = _userManager.GetUserId(User)!;
        IsSignedIn = _signInManager.IsSignedIn(User);

        var isAuthorized = User.IsInRole(Constants.DocumentAdministratorsRole) || User.IsInRole(Constants.DocumentManagersRole);

        // if not authorized only show owned and shared documents
        // otherwise show all documents
        if (!isAuthorized)
        {
            var ownedDocuments = _context.Documents
                .Where(d => d.OwnerId == userId && d.Status != DocumentStatus.Rejected && EF.Functions.Like(d.FileName, $"%{Search}%"));

            var sharedDocuments = _context.UserDocuments
                .Where(ud => ud.UserId == userId && ud.Document.Status != DocumentStatus.Rejected && EF.Functions.Like(ud.Document.FileName, $"%{Search}%"))
                .Select(ud => ud.Document);

            Documents = ownedDocuments
                .Union(sharedDocuments)
                .Distinct()
                .Include(d => d.Owner)
                .ToList();
        }
        else
        {
            Documents = _context.Documents
                .Include(d => d.Owner)
                .Where(d => EF.Functions.Like(d.FileName, $"%{Search}%"))
                .ToList();
        }
    }
}

