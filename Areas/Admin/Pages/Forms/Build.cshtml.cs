using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ufas1Forms.Data;
using Ufas1Forms.Models;

namespace Ufas1Forms.Areas.Admin.Pages.Forms;

[Authorize(Roles = "admin,facadmin")]
public partial class BuildModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public BuildModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public Form Form { get; set; } = new();
    public List<FormField> Fields { get; set; } = new();
    public List<FormField> SelectFields { get; set; } = new();

    [BindProperty]
    public FieldInputModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var form = await GetFormAsync(id);
        if (form == null)
            return NotFound();

        Form = form;
        Fields = form.Fields.OrderBy(f => f.Order).ToList();
        SelectFields = Fields.Where(f => f.FieldType == FieldType.Select).ToList();
        return Page();
    }

    public async Task<IActionResult> OnPostSaveFieldAsync(int id)
    {
        var form = await GetFormAsync(id);
        if (form == null)
            return NotFound();

        Form = form;
        Fields = form.Fields.OrderBy(f => f.Order).ToList();
        SelectFields = Fields.Where(f => f.FieldType == FieldType.Select).ToList();

        if (!ModelState.IsValid)
            return Page();

        if (Input.FieldId.HasValue)
        {
            // Edit existing field
            var field = await _context.FormFields.FirstOrDefaultAsync(f => f.Id == Input.FieldId && f.FormId == id);
            if (field == null)
                return NotFound();

            field.Label = Input.Label;
            field.Name = GenerateUniqueName(Input.Label, id, field.Id);
            field.FieldType = Input.FieldType;
            field.HelpText = Input.HelpText;
            field.Placeholder = Input.Placeholder;
            field.DefaultValue = Input.DefaultValue;
            field.IsRequired = Input.IsRequired;
            field.OptionsJson = Input.OptionsJson;
            field.ParentFieldId = Input.ParentFieldId;
        }
        else
        {
            // Add new field
            var maxOrder = form.Fields.Any() ? form.Fields.Max(f => f.Order) : 0;

            var field = new FormField
            {
                FormId = id,
                Label = Input.Label,
                Name = GenerateUniqueName(Input.Label, id, null),
                FieldType = Input.FieldType,
                HelpText = Input.HelpText,
                Placeholder = Input.Placeholder,
                DefaultValue = Input.DefaultValue,
                IsRequired = Input.IsRequired,
                OptionsJson = Input.OptionsJson,
                ParentFieldId = Input.ParentFieldId,
                Order = maxOrder + 1
            };

            _context.FormFields.Add(field);
        }

        await _context.SaveChangesAsync();
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostDeleteFieldAsync(int id, int fieldId)
    {
        var form = await GetFormAsync(id);
        if (form == null)
            return NotFound();

        var field = await _context.FormFields.FirstOrDefaultAsync(f => f.Id == fieldId && f.FormId == id);
        if (field != null)
        {
            _context.FormFields.Remove(field);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostMoveUpAsync(int id, int fieldId)
    {
        var fields = await _context.FormFields
            .Where(f => f.FormId == id)
            .OrderBy(f => f.Order)
            .ToListAsync();

        var idx = fields.FindIndex(f => f.Id == fieldId);
        if (idx > 0)
        {
            (fields[idx].Order, fields[idx - 1].Order) = (fields[idx - 1].Order, fields[idx].Order);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostMoveDownAsync(int id, int fieldId)
    {
        var fields = await _context.FormFields
            .Where(f => f.FormId == id)
            .OrderBy(f => f.Order)
            .ToListAsync();

        var idx = fields.FindIndex(f => f.Id == fieldId);
        if (idx >= 0 && idx < fields.Count - 1)
        {
            (fields[idx].Order, fields[idx + 1].Order) = (fields[idx + 1].Order, fields[idx].Order);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage(new { id });
    }

    private async Task<Form?> GetFormAsync(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var isAdmin = User.IsInRole("admin");

        var query = _context.Forms
            .Include(f => f.Fields)
            .AsQueryable();

        if (!isAdmin && currentUser?.FaculteId != null)
        {
            query = query.Where(f => f.FaculteId == currentUser.FaculteId);
        }

        return await query.FirstOrDefaultAsync(f => f.Id == id);
    }

    private string GenerateUniqueName(string label, int formId, int? excludeFieldId)
    {
        var baseName = GenerateFieldName(label);
        var name = baseName;
        var counter = 1;

        var existingNames = _context.FormFields
            .Where(f => f.FormId == formId && f.Id != excludeFieldId)
            .Select(f => f.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        while (existingNames.Contains(name))
        {
            name = $"{baseName}_{counter++}";
        }

        return name;
    }

    private static string GenerateFieldName(string label)
    {
        var name = label.ToLowerInvariant();
        name = NonAlphaNumericRegex().Replace(name, "_");
        name = MultipleUnderscoreRegex().Replace(name, "_");
        return name.Trim('_');
    }

    [GeneratedRegex(@"[^a-z0-9]+")]
    private static partial Regex NonAlphaNumericRegex();

    [GeneratedRegex(@"_+")]
    private static partial Regex MultipleUnderscoreRegex();

    public class FieldInputModel
    {
        public int? FieldId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Label { get; set; } = string.Empty;

        public FieldType FieldType { get; set; } = FieldType.Text;

        [MaxLength(500)]
        public string? HelpText { get; set; }

        [MaxLength(255)]
        public string? Placeholder { get; set; }

        [MaxLength(500)]
        public string? DefaultValue { get; set; }

        public bool IsRequired { get; set; }

        public string? OptionsJson { get; set; }

        public int? ParentFieldId { get; set; }
    }
}
