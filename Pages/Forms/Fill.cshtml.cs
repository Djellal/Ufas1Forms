using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ufas1Forms.Data;
using Ufas1Forms.Models;

namespace Ufas1Forms.Pages.Forms;

public class FillModel(ApplicationDbContext context) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Slug { get; set; } = string.Empty;

    public Form? Form { get; set; }

    [BindProperty]
    public Dictionary<string, string> Answers { get; set; } = new();

    [BindProperty]
    public Dictionary<string, List<string>> CheckboxAnswers { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        Form = await context.Forms
            .Include(f => f.Fields.OrderBy(field => field.Order))
            .FirstOrDefaultAsync(f => f.Slug == Slug && f.Status == FormStatus.Published);

        if (Form == null)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Form = await context.Forms
            .Include(f => f.Fields.OrderBy(field => field.Order))
            .FirstOrDefaultAsync(f => f.Slug == Slug && f.Status == FormStatus.Published);

        if (Form == null)
            return NotFound();

        foreach (var field in Form.Fields)
        {
            if (field.IsRequired)
            {
                if (field.FieldType == FieldType.Checkbox)
                {
                    if (!CheckboxAnswers.ContainsKey(field.Name) || !CheckboxAnswers[field.Name].Any())
                    {
                        ModelState.AddModelError($"CheckboxAnswers[{field.Name}]", $"{field.Label} is required.");
                    }
                }
                else
                {
                    if (!Answers.ContainsKey(field.Name) || string.IsNullOrWhiteSpace(Answers[field.Name]))
                    {
                        ModelState.AddModelError($"Answers[{field.Name}]", $"{field.Label} is required.");
                    }
                }
            }
        }

        if (!ModelState.IsValid)
            return Page();

        int? faculteId = null;
        int? domaineId = null;

        var faculteField = Form.Fields.FirstOrDefault(f => 
            f.Name.Equals("faculte", StringComparison.OrdinalIgnoreCase) || 
            f.Name.Equals("faculteid", StringComparison.OrdinalIgnoreCase));
        if (faculteField != null && Answers.TryGetValue(faculteField.Name, out var faculteValue) && int.TryParse(faculteValue, out var fId))
        {
            faculteId = fId;
        }

        var domaineField = Form.Fields.FirstOrDefault(f => 
            f.Name.Equals("domaine", StringComparison.OrdinalIgnoreCase) || 
            f.Name.Equals("domaineid", StringComparison.OrdinalIgnoreCase));
        if (domaineField != null && Answers.TryGetValue(domaineField.Name, out var domaineValue) && int.TryParse(domaineValue, out var dId))
        {
            domaineId = dId;
        }

        var submission = new FormSubmission
        {
            FormId = Form.Id,
            SubmittedAt = DateTime.UtcNow,
            SubmittedByUserId = User.Identity?.IsAuthenticated == true ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = Request.Headers.UserAgent.ToString().Length > 500 
                ? Request.Headers.UserAgent.ToString()[..500] 
                : Request.Headers.UserAgent.ToString(),
            Status = SubmissionStatus.Completed,
            FaculteId = faculteId,
            DomaineId = domaineId
        };

        context.FormSubmissions.Add(submission);
        await context.SaveChangesAsync();

        foreach (var field in Form.Fields)
        {
            var answer = new FormAnswer
            {
                SubmissionId = submission.Id,
                FieldId = field.Id
            };

            if (field.FieldType == FieldType.Checkbox)
            {
                if (CheckboxAnswers.TryGetValue(field.Name, out var values) && values.Any())
                {
                    answer.ValueJson = JsonSerializer.Serialize(values);
                }
            }
            else if (field.FieldType == FieldType.Hidden)
            {
                answer.ValueText = field.DefaultValue;
            }
            else
            {
                if (Answers.TryGetValue(field.Name, out var value))
                {
                    answer.ValueText = value;
                }
            }

            context.FormAnswers.Add(answer);
        }

        await context.SaveChangesAsync();

        return RedirectToPage("./Thanks", new { slug = Slug });
    }
}
