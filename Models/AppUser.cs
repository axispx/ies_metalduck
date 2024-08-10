using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace metalduck.Models
{
    public class AppUser : IdentityUser
    {
        public ICollection<Document> OwnedDocuments { get; set; }
        //public ICollection<Document> SharedDocuments { get; set; }
    }
}