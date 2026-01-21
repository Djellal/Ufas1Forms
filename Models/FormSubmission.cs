using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ufas1Forms.Models;

public enum SubmissionStatus
{
    Completed,
    Partial,
    Deleted
}

public class FormSubmission
{
    public int Id { get; set; }

    public int FormId { get; set; }

    [ForeignKey("FormId")]
    public Form? Form { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(450)]
    public string? SubmittedByUserId { get; set; }

    public ApplicationUser? SubmittedByUser { get; set; }

    [MaxLength(45)]
    public string? IpAddress { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }

    public SubmissionStatus Status { get; set; } = SubmissionStatus.Completed;

    public ICollection<FormAnswer> Answers { get; set; } = new List<FormAnswer>();

    public ICollection<UploadedFile> Files { get; set; } = new List<UploadedFile>();
}
