using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ufas1Forms.Models;

namespace Ufas1Forms.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Etablissement> Etablissements { get; set; }
    public DbSet<Faculte> Facultes { get; set; }
    public DbSet<Domaine> Domaines { get; set; }
    public DbSet<Form> Forms { get; set; }
    public DbSet<FormField> FormFields { get; set; }
    public DbSet<FormSubmission> FormSubmissions { get; set; }
    public DbSet<FormAnswer> FormAnswers { get; set; }
    public DbSet<UploadedFile> UploadedFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Form>(entity =>
        {
            entity.HasIndex(f => f.Slug).IsUnique();
            entity.HasOne(f => f.CreatedByUser)
                  .WithMany()
                  .HasForeignKey(f => f.CreatedByUserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<FormField>(entity =>
        {
            entity.HasIndex(f => new { f.FormId, f.Name }).IsUnique();
            entity.HasIndex(f => new { f.FormId, f.Order });
            entity.HasOne(f => f.Form)
                  .WithMany(form => form.Fields)
                  .HasForeignKey(f => f.FormId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(f => f.ParentField)
                  .WithMany(p => p.ChildFields)
                  .HasForeignKey(f => f.ParentFieldId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<FormSubmission>(entity =>
        {
            entity.HasIndex(s => new { s.FormId, s.SubmittedAt });
            entity.HasIndex(s => s.FaculteId);
            entity.HasIndex(s => s.DomaineId);
            entity.HasOne(s => s.Form)
                  .WithMany(f => f.Submissions)
                  .HasForeignKey(s => s.FormId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(s => s.SubmittedByUser)
                  .WithMany()
                  .HasForeignKey(s => s.SubmittedByUserId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(s => s.Faculte)
                  .WithMany()
                  .HasForeignKey(s => s.FaculteId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(s => s.Domaine)
                  .WithMany()
                  .HasForeignKey(s => s.DomaineId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<FormAnswer>(entity =>
        {
            entity.HasIndex(a => a.SubmissionId);
            entity.HasIndex(a => a.FieldId);
            entity.HasOne(a => a.Submission)
                  .WithMany(s => s.Answers)
                  .HasForeignKey(a => a.SubmissionId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(a => a.Field)
                  .WithMany(f => f.Answers)
                  .HasForeignKey(a => a.FieldId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UploadedFile>(entity =>
        {
            entity.HasOne(f => f.Submission)
                  .WithMany(s => s.Files)
                  .HasForeignKey(f => f.SubmissionId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(f => f.Field)
                  .WithMany()
                  .HasForeignKey(f => f.FieldId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
