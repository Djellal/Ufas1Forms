using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ufas1Forms.Data;
using Ufas1Forms.Models;

namespace Ufas1Forms.Areas.Admin.Pages.Forms;

[Authorize(Roles = "admin,facadmin")]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<SelectListItem> FaculteOptions { get; set; } = new();
    public bool IsAdmin { get; set; }

    public async Task OnGetAsync()
    {
        IsAdmin = User.IsInRole("admin");
        if (IsAdmin)
        {
            FaculteOptions = await _context.Facultes
                .OrderBy(f => f.Nom)
                .Select(f => new SelectListItem { Value = f.Id.ToString(), Text = f.Nom })
                .ToListAsync();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            IsAdmin = User.IsInRole("admin");
            if (IsAdmin)
            {
                FaculteOptions = await _context.Facultes
                    .OrderBy(f => f.Nom)
                    .Select(f => new SelectListItem { Value = f.Id.ToString(), Text = f.Nom })
                    .ToListAsync();
            }
            return Page();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var isAdmin = User.IsInRole("admin");

        var form = new Form
        {
            Title = Input.Title,
            Description = Input.Description,
            Type = Input.Type,
            Slug = Input.Slug,
            Status = FormStatus.Draft,
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            FaculteId = isAdmin ? Input.FaculteId : currentUser?.FaculteId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Forms.Add(form);
        await _context.SaveChangesAsync();

        return RedirectToPage("Build", new { id = form.Id });
    }

    public class InputModel
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public FormType Type { get; set; } = FormType.Survey;

        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug must be lowercase letters, numbers, and hyphens only.")]
        public string Slug { get; set; } = string.Empty;

        [Display(Name = "Faculty")]
        public int? FaculteId { get; set; }
    }
}
