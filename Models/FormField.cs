using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ufas1Forms.Models;

public enum FieldType
{
    Text,
    Email,
    Number,
    Textarea,
    Select,
    Radio,
    Checkbox,
    Date,
    Time,
    Phone,
    Url,
    Password,
    Hidden,
    File
}

public class FormField
{
    public int Id { get; set; }

    public int FormId { get; set; }

    [ForeignKey("FormId")]
    public Form? Form { get; set; }

    public int? ParentFieldId { get; set; }

    [ForeignKey("ParentFieldId")]
    public FormField? ParentField { get; set; }

    public ICollection<FormField> ChildFields { get; set; } = new List<FormField>();

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Label { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? HelpText { get; set; }

    public FieldType FieldType { get; set; } = FieldType.Text;

    public bool IsRequired { get; set; }

    public int Order { get; set; }

    [MaxLength(255)]
    public string? Placeholder { get; set; }

    [MaxLength(500)]
    public string? DefaultValue { get; set; }

    public string? ValidationJson { get; set; }

    public string? OptionsJson { get; set; }

    public string? FileConstraintsJson { get; set; }

    public ICollection<FormAnswer> Answers { get; set; } = new List<FormAnswer>();
}
