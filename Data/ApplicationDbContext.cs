using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using metalduck.Models;

namespace metalduck.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Document> Documents { get; set; }
    public DbSet<UserDocument> UserDocuments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Document>()
            .HasOne(d => d.Owner)
            .WithMany()
            .HasForeignKey(d => d.OwnerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<UserDocument>()
            .HasKey(ud => new { ud.UserId, ud.DocumentId });

        builder.Entity<UserDocument>()
            .HasOne(ud => ud.User)
            .WithMany()
            .HasForeignKey(ud => ud.UserId);

        builder.Entity<UserDocument>()
            .HasOne(ud => ud.Document)
            .WithMany()
            .HasForeignKey(ud => ud.DocumentId);
    }
}

