using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Ufas1Forms.Data;
using Ufas1Forms.Models;

namespace Ufas1Forms.Areas.Admin.Pages.Forms;

[Authorize(Roles = "admin,facadmin")]
public partial class BuildModel(ApplicationDbContext context) : PageModel
{
    public Form Form { get; set; } = default!;
    public List<FormField> Fields { get; set; } = [];

    [BindProperty]
    public FormFieldInput FieldInput { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var form = await context.Forms
            .Include(f => f.Fields.OrderBy(ff => ff.Order))
            .FirstOrDefaultAsync(f => f.Id == id);

        if (form is null)
            return NotFound();

        Form = form;
        Fields = form.Fields.ToList();
        return Page();
    }

    public async Task<IActionResult> OnPostAddFieldAsync(int id)
    {
        var form = await context.Forms
            .Include(f => f.Fields)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (form is null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            Form = form;
            Fields = form.Fields.OrderBy(f => f.Order).ToList();
            return Page();
        }

        var name = GenerateFieldName(FieldInput.Label);
        if (form.Fields.Any(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            name = $"{name}_{form.Fields.Count + 1}";
        }

        var maxOrder = form.Fields.Any() ? form.Fields.Max(f => f.Order) : 0;

        var field = new FormField
        {
            FormId = id,
            Label = FieldInput.Label,
            Name = name,
            FieldType = FieldInput.FieldType,
            HelpText = FieldInput.HelpText,
            Placeholder = FieldInput.Placeholder,
            DefaultValue = FieldInput.DefaultValue,
            IsRequired = FieldInput.IsRequired,
            OptionsJson = FieldInput.OptionsJson,
            ParentFieldId = FieldInput.ParentFieldId,
            Order = maxOrder + 1
        };

        context.FormFields.Add(field);
        await context.SaveChangesAsync();

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostEditFieldAsync(int id, int fieldId)
    {
        var field = await context.FormFields
            .Include(f => f.Form)
            .ThenInclude(f => f!.Fields)
            .FirstOrDefaultAsync(f => f.Id == fieldId && f.FormId == id);

        if (field is null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            Form = field.Form!;
            Fields = field.Form!.Fields.OrderBy(f => f.Order).ToList();
            return Page();
        }

        var name = GenerateFieldName(FieldInput.Label);
        if (field.Form!.Fields.Any(f => f.Id != fieldId && f.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            name = $"{name}_{fieldId}";
        }

        field.Label = FieldInput.Label;
        field.Name = name;
        field.FieldType = FieldInput.FieldType;
        field.HelpText = FieldInput.HelpText;
        field.Placeholder = FieldInput.Placeholder;
        field.DefaultValue = FieldInput.DefaultValue;
        field.IsRequired = FieldInput.IsRequired;
        field.OptionsJson = FieldInput.OptionsJson;
        field.ParentFieldId = FieldInput.ParentFieldId;

        await context.SaveChangesAsync();

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostDeleteFieldAsync(int id, int fieldId)
    {
        var field = await context.FormFields
            .FirstOrDefaultAsync(f => f.Id == fieldId && f.FormId == id);

        if (field is null)
            return NotFound();

        context.FormFields.Remove(field);
        await context.SaveChangesAsync();

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostMoveUpAsync(int id, int fieldId)
    {
        var fields = await context.FormFields
            .Where(f => f.FormId == id)
            .OrderBy(f => f.Order)
            .ToListAsync();

        var currentIndex = fields.FindIndex(f => f.Id == fieldId);
        if (currentIndex <= 0)
            return RedirectToPage(new { id });

        var current = fields[currentIndex];
        var previous = fields[currentIndex - 1];

        (current.Order, previous.Order) = (previous.Order, current.Order);

        await context.SaveChangesAsync();

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostMoveDownAsync(int id, int fieldId)
    {
        var fields = await context.FormFields
            .Where(f => f.FormId == id)
            .OrderBy(f => f.Order)
            .ToListAsync();

        var currentIndex = fields.FindIndex(f => f.Id == fieldId);
        if (currentIndex < 0 || currentIndex >= fields.Count - 1)
            return RedirectToPage(new { id });

        var current = fields[currentIndex];
        var next = fields[currentIndex + 1];

        (current.Order, next.Order) = (next.Order, current.Order);

        await context.SaveChangesAsync();

        return RedirectToPage(new { id });
    }

    private static string GenerateFieldName(string label)
    {
        var name = label.ToLowerInvariant();
        name = FieldNameRegex().Replace(name, "_");
        name = MultipleUnderscoreRegex().Replace(name, "_");
        return name.Trim('_');
    }

    [GeneratedRegex(@"[^a-z0-9]+")]
    private static partial Regex FieldNameRegex();

    [GeneratedRegex(@"_+")]
    private static partial Regex MultipleUnderscoreRegex();
}

public class FormFieldInput
{
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.MaxLength(255)]
    public string Label { get; set; } = string.Empty;

    public FieldType FieldType { get; set; } = FieldType.Text;

    [System.ComponentModel.DataAnnotations.MaxLength(500)]
    public string? HelpText { get; set; }

    [System.ComponentModel.DataAnnotations.MaxLength(255)]
    public string? Placeholder { get; set; }

    [System.ComponentModel.DataAnnotations.MaxLength(500)]
    public string? DefaultValue { get; set; }

    public bool IsRequired { get; set; }

    public string? OptionsJson { get; set; }

    public int? ParentFieldId { get; set; }
}
