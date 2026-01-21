using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ufas1Forms.Data;
using Ufas1Forms.Models;

namespace Ufas1Forms.Areas.Admin.Pages.Forms;

[Authorize(Roles = "admin,facadmin")]
public class SubmissionDetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public SubmissionDetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Form Form { get; set; } = default!;
    public FormSubmission Submission { get; set; } = default!;
    public string? SubmittedByEmail { get; set; }
    public List<AnswerDisplayItem> Answers { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int formId, int submissionId)
    {
        var form = await _context.Forms
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == formId);

        if (form == null)
        {
            return NotFound();
        }

        Form = form;

        var submission = await _context.FormSubmissions
            .Include(s => s.SubmittedByUser)
            .Include(s => s.Answers)
                .ThenInclude(a => a.Field)
            .FirstOrDefaultAsync(s => s.Id == submissionId && s.FormId == formId);

        if (submission == null)
        {
            return NotFound();
        }

        Submission = submission;
        SubmittedByEmail = submission.SubmittedByUser?.Email;

        Answers = submission.Answers
            .Where(a => a.Field != null)
            .OrderBy(a => a.Field!.Order)
            .Select(a => new AnswerDisplayItem
            {
                Label = a.Field!.Label,
                Value = GetDisplayValue(a)
            })
            .ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int formId, int submissionId)
    {
        var submission = await _context.FormSubmissions.FindAsync(submissionId);
        if (submission != null)
        {
            _context.FormSubmissions.Remove(submission);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage("Submissions", new { id = formId });
    }

    private static string GetDisplayValue(FormAnswer answer)
    {
        if (!string.IsNullOrEmpty(answer.ValueJson))
        {
            try
            {
                var values = JsonSerializer.Deserialize<List<string>>(answer.ValueJson);
                if (values != null)
                {
                    return string.Join(", ", values);
                }
            }
            catch
            {
                return answer.ValueJson;
            }
        }

        return answer.ValueText ?? string.Empty;
    }

    public class AnswerDisplayItem
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
