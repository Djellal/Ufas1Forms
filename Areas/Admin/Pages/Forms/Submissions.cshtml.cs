using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ufas1Forms.Data;
using Ufas1Forms.Models;

namespace Ufas1Forms.Areas.Admin.Pages.Forms;

[Authorize(Roles = "admin,facadmin")]
public class SubmissionsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public SubmissionsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public Form Form { get; set; } = default!;
    public List<SubmissionListItem> Submissions { get; set; } = new();
    public int TotalCount { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var isAdmin = User.IsInRole("admin");

        var formQuery = _context.Forms.AsNoTracking();

        if (!isAdmin && currentUser?.FaculteId != null)
        {
            formQuery = formQuery.Where(f => f.FaculteId == currentUser.FaculteId);
        }

        var form = await formQuery.FirstOrDefaultAsync(f => f.Id == id);

        if (form == null)
        {
            return NotFound();
        }

        Form = form;

        Submissions = await _context.FormSubmissions
            .Where(s => s.FormId == id)
            .Include(s => s.SubmittedByUser)
            .OrderByDescending(s => s.SubmittedAt)
            .Select(s => new SubmissionListItem
            {
                Id = s.Id,
                SubmittedAt = s.SubmittedAt,
                SubmittedByEmail = s.SubmittedByUser != null ? s.SubmittedByUser.Email : null,
                Status = s.Status
            })
            .ToListAsync();

        TotalCount = Submissions.Count;

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id, int submissionId)
    {
        var submission = await _context.FormSubmissions.FindAsync(submissionId);
        if (submission != null)
        {
            _context.FormSubmissions.Remove(submission);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage(new { id });
    }

    public class SubmissionListItem
    {
        public int Id { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string? SubmittedByEmail { get; set; }
        public SubmissionStatus Status { get; set; }
    }
}
