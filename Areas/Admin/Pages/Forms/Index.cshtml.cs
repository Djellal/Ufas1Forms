using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ufas1Forms.Data;
using Ufas1Forms.Models;

namespace Ufas1Forms.Areas.Admin.Pages.Forms;

[Authorize(Roles = "admin,facadmin")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<FormListItem> Forms { get; set; } = new();

    public async Task OnGetAsync()
    {
        Forms = await _context.Forms
            .Select(f => new FormListItem
            {
                Id = f.Id,
                Title = f.Title,
                Type = f.Type,
                Status = f.Status,
                CreatedAt = f.CreatedAt,
                SubmissionCount = f.Submissions.Count
            })
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var form = await _context.Forms.FindAsync(id);
        if (form != null)
        {
            _context.Forms.Remove(form);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public class FormListItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public FormType Type { get; set; }
        public FormStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int SubmissionCount { get; set; }
    }
}
