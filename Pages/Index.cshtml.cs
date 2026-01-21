using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ufas1Forms.Data;
using Ufas1Forms.Models;
using Microsoft.EntityFrameworkCore;

namespace Ufas1Forms.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Form> ActiveForms { get; set; } = new List<Form>();

    public async Task<IActionResult> OnGetAsync()
    {
        // Fetch published forms (active forms)
        ActiveForms = await _context.Forms
            .Where(f => f.Status == FormStatus.Published)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();

        return Page();
    }
}
