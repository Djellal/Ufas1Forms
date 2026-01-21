using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ufas1Forms.Data;
using Ufas1Forms.Models;

namespace Ufas1Forms.Pages.Forms;

public class ThanksModel(ApplicationDbContext context) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Slug { get; set; } = string.Empty;

    public Form? Form { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Form = await context.Forms
            .FirstOrDefaultAsync(f => f.Slug == Slug && f.Status == FormStatus.Published);

        if (Form == null)
            return NotFound();

        return Page();
    }
}
