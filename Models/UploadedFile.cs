using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ufas1Forms.Models;

public class UploadedFile
{
    public int Id { get; set; }

    public int SubmissionId { get; set; }

    [ForeignKey("SubmissionId")]
    public FormSubmission? Submission { get; set; }

    public int FieldId { get; set; }

    [ForeignKey("FieldId")]
    public FormField? Field { get; set; }

    [Required]
    [MaxLength(255)]
    public string OriginalFileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string StoredFileName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ContentType { get; set; }

    public long SizeBytes { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
