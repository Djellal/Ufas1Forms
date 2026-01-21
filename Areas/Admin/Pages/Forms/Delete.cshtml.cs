using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ufas1Forms.Data;
using Ufas1Forms.Models;

namespace Ufas1Forms.Areas.Admin.Pages.Forms;

[Authorize(Roles = "admin,facadmin")]
public class DeleteModel(ApplicationDbContext context) : PageModel
{
    [BindProperty]
    public Form Form { get; set; } = default!;

    public int FieldCount { get; set; }
    public int SubmissionCount { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var form = await context.Forms
            .Include(f => f.Fields)
            .Include(f => f.Submissions)
            .FirstOrDefaultAsync(f => f.Id == id);

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
        var form = await context.Forms.FindAsync(id);

        if (form == null)
        {
            return NotFound();
        }

        context.Forms.Remove(form);
        await context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
