using System.ComponentModel.DataAnnotations.Schema;

namespace Ufas1Forms.Models;

public class FormAnswer
{
    public int Id { get; set; }

    public int SubmissionId { get; set; }

    [ForeignKey("SubmissionId")]
    public FormSubmission? Submission { get; set; }

    public int FieldId { get; set; }

    [ForeignKey("FieldId")]
    public FormField? Field { get; set; }

    public string? ValueText { get; set; }

    public string? ValueJson { get; set; }
}
