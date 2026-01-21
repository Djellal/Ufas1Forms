using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ufas1Forms.Data;
using Ufas1Forms.Models;

namespace Ufas1Forms.Areas.Admin.Pages.Forms;

[Authorize(Roles = "admin,facadmin")]
public class EditModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : PageModel
{
    [BindProperty]
    public Form Form { get; set; } = default!;

    public int FieldCount { get; set; }
    public int SubmissionCount { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var currentUser = await userManager.GetUserAsync(User);
        var isAdmin = User.IsInRole("admin");

        var query = context.Forms
            .Include(f => f.Fields)
            .Include(f => f.Submissions)
            .AsQueryable();

        if (!isAdmin && currentUser?.FaculteId != null)
        {
            query = query.Where(f => f.FaculteId == currentUser.FaculteId);
        }

        var form = await query.FirstOrDefaultAsync(f => f.Id == id);

        if (form == null)
        {
            return NotFound();
        }

        Form = form;
        FieldCount = form.Fields.Count;
        SubmissionCount = form.Submissions.Count;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var currentUser = await userManager.GetUserAsync(User);
        var isAdmin = User.IsInRole("admin");

        var form = await context.Forms.FindAsync(id);

        if (form == null)
        {
            return NotFound();
        }

        if (!isAdmin && currentUser?.FaculteId != null && form.FaculteId != currentUser.FaculteId)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            FieldCount = await context.FormFields.CountAsync(f => f.FormId == id);
            SubmissionCount = await context.FormSubmissions.CountAsync(s => s.FormId == id);
            return Page();
        }

        form.Title = Form.Title;
        form.Description = Form.Description;
        form.Type = Form.Type;
        form.Status = Form.Status;
        form.Slug = Form.Slug;
        form.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return RedirectToPage("./Edit", new { id });
    }
}
