using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using metalduck.Models;
using metalduck.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using metalduck.Authorization;

namespace metalduck.Pages
{
	public class DocumentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AppIndexModel> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuthorizationService _auth;

        public DocumentModel(ApplicationDbContext context, ILogger<AppIndexModel> logger, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IAuthorizationService auth)
        {
            _context = context;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _auth = auth;
        }

        public Document Document { get; set; }
        public string CurrentUserId { get; set; }
        public IList<IdentityUser> Users { get; set; }
        public IList<UserDocument> UserDocuments { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public async void OnGet()
        {
            CurrentUserId = _userManager.GetUserId(User)!;

            var document = from d in _context.Documents select d;
            document = document.Include(d => d.Owner).Where(d => d.Id == Id);

            Document = document.First();

            Users = await _userManager.GetUsersInRoleAsync("Member");

            UserDocuments = _context.UserDocuments.Where(u => u.DocumentId == Id).ToList();
        }

        public async Task<IActionResult> OnPostApprove()
        {
            Document = _context.Documents.Where(d => d.Id == Id).First();

            var isAuthorized = await _auth.AuthorizeAsync(User, Document, DocumentOperations.Approve);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Document.Status = DocumentStatus.Approved;

            _context.Documents.Update(Document);
            _context.SaveChanges();

            return RedirectToPage("/Document", new { id = Document.Id });
        }

        public async Task<IActionResult> OnPostReject()
        {
            Document = _context.Documents.Where(d => d.Id == Id).First();

            var isAuthorized = await _auth.AuthorizeAsync(User, Document, DocumentOperations.Reject);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Document.Status = DocumentStatus.Approved;
            Document.Status = DocumentStatus.Rejected;

            _context.Documents.Update(Document);
            _context.SaveChanges();

            return RedirectToPage("/Document", new { id = Document.Id });

        }

        public async Task<IActionResult> OnPostDelete()
        {
            Document = _context.Documents.Where(d => d.Id == Id).First();

            var isAuthorized = await _auth.AuthorizeAsync(User, Document, DocumentOperations.Delete);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            _context.Documents.Remove(Document);
            _context.SaveChanges();

            var filePath = Path.Combine("wwwroot/uploads", Document.FileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return RedirectToPage("/Index");
        }

        [BindProperty]
        public string AddSharedUser { get; set; }

        public async Task<IActionResult> OnPostShare()
        {
            Document = _context.Documents.Where(d => d.Id == Id).First();
            UserDocument userDocument = new UserDocument
            {
                UserId = AddSharedUser,
                DocumentId = Id,
            };

            var isAuthorized = await _auth.AuthorizeAsync(User, Document, DocumentOperations.Share);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            _context.UserDocuments.Add(userDocument);
            _context.SaveChanges();

            return RedirectToPage("/Document", new { id = Id });
        }

        [BindProperty]
        public string RemoveSharedUser { get; set; }

        public async Task<IActionResult> OnPostRemoveShare()
        {
            Document = _context.Documents.Where(d => d.Id == Id).First();
            var userDocument = _context.UserDocuments.First(u => u.UserId == RemoveSharedUser && u.DocumentId == Id);

            var isAuthorized = await _auth.AuthorizeAsync(User, Document, DocumentOperations.Share);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            _context.UserDocuments.Remove(userDocument);
            _context.SaveChanges();

            return RedirectToPage("/Document", new { id = Id });
        }
    }
}
