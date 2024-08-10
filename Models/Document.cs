using Microsoft.AspNetCore.Identity;

namespace metalduck.Models
{
    public class Document
    {
        public int Id { get; set; }
        public required string FileName { get; set; }
        public required DateTime DateCreated { get; set; }
        public DocumentStatus Status {get; set;}

        public required string OwnerId { get; set; }

        public IdentityUser Owner { get; set; }
    }

    public class UserDocument
    {
        public required string UserId { get; set; }
        public required int DocumentId { get; set; }

        public IdentityUser User { get; set; }
        public Document Document { get; set; }
    }
}

public enum DocumentStatus
{
    Submitted,
    Approved,
    Rejected,
}