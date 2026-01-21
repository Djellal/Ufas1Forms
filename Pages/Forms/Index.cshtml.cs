using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ufas1Forms.Data;
using Ufas1Forms.Models;

namespace Ufas1Forms.Pages.Forms;

public class IndexModel(ApplicationDbContext context) : PageModel
{
    public List<Form> Forms { get; set; } = new();

    public async Task OnGetAsync()
    {
        Forms = await context.Forms
            .Where(f => f.Status == FormStatus.Published)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }
}
