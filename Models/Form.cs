using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Ufas1Forms.Models;

public enum FormType
{
    Registration,
    Survey,
    DataCollection
}

public enum FormStatus
{
    Draft,
    Published,
    Archived
}

public class Form
{
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public FormType Type { get; set; } = FormType.Survey;

    public FormStatus Status { get; set; } = FormStatus.Draft;

    [Required]
    [MaxLength(100)]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(450)]
    public string? CreatedByUserId { get; set; }

    public IdentityUser? CreatedByUser { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<FormField> Fields { get; set; } = new List<FormField>();

    public ICollection<FormSubmission> Submissions { get; set; } = new List<FormSubmission>();
}
