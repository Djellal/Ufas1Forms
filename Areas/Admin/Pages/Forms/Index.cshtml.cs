using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<FormListItem> Forms { get; set; } = new();

    public async Task OnGetAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var isAdmin = User.IsInRole("admin");

        var query = _context.Forms.AsQueryable();

        if (!isAdmin && currentUser?.FaculteId != null)
        {
            query = query.Where(f => f.FaculteId == currentUser.FaculteId);
        }

        Forms = await query
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
        var currentUser = await _userManager.GetUserAsync(User);
        var isAdmin = User.IsInRole("admin");

        var form = await _context.Forms.FindAsync(id);
        if (form != null)
        {
            if (!isAdmin && currentUser?.FaculteId != null && form.FaculteId != currentUser.FaculteId)
            {
                return Forbid();
            }
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
