using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ufas1Forms.Data;
using Ufas1Forms.Models;

namespace Ufas1Forms.Areas.Admin.Pages.Forms;

[Authorize(Roles = "admin,facadmin")]
public class ExportCsvModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public ExportCsvModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var form = await _context.Forms
            .Include(f => f.Fields.OrderBy(field => field.Order))
            .FirstOrDefaultAsync(f => f.Id == id);

        if (form == null)
        {
            return NotFound();
        }

        var submissions = await _context.FormSubmissions
            .Where(s => s.FormId == id)
            .Include(s => s.SubmittedByUser)
            .Include(s => s.Answers)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();

        var csv = new StringBuilder();

        var headers = new List<string> { "SubmissionId", "SubmittedAt", "SubmittedBy" };
        foreach (var field in form.Fields)
        {
            headers.Add(field.Label);
        }
        csv.AppendLine(string.Join(",", headers.Select(EscapeCsvField)));

        foreach (var submission in submissions)
        {
            var row = new List<string>
            {
                submission.Id.ToString(),
                submission.SubmittedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                submission.SubmittedByUser?.Email ?? "Anonymous"
            };

            foreach (var field in form.Fields)
            {
                var answer = submission.Answers.FirstOrDefault(a => a.FieldId == field.Id);
                var value = GetCsvValue(answer);
                row.Add(value);
            }

            csv.AppendLine(string.Join(",", row.Select(EscapeCsvField)));
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        var filename = $"{form.Slug}-submissions.csv";

        return File(bytes, "text/csv", filename);
    }

    private static string GetCsvValue(FormAnswer? answer)
    {
        if (answer == null)
        {
            return string.Empty;
        }

        if (!string.IsNullOrEmpty(answer.ValueJson))
        {
            try
            {
                var values = JsonSerializer.Deserialize<List<string>>(answer.ValueJson);
                if (values != null)
                {
                    return string.Join(";", values);
                }
            }
            catch
            {
                return answer.ValueJson;
            }
        }

        return answer.ValueText ?? string.Empty;
    }

    private static string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field))
        {
            return string.Empty;
        }

        if (field.Contains('"') || field.Contains(',') || field.Contains('\n') || field.Contains('\r'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }

        return field;
    }
}
