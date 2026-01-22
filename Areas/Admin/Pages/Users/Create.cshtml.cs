using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ufas1Forms.Data;
using Ufas1Forms.Models;

namespace Ufas1Forms.Areas.Admin.Pages.Users;

[Authorize(Roles = "admin")]
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

    public List<SelectListItem> RoleOptions { get; set; } = new();
    public List<SelectListItem> FaculteOptions { get; set; } = new();

    public async Task OnGetAsync()
    {
        await LoadOptionsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadOptionsAsync();

        if (!ModelState.IsValid)
            return Page();

        var user = new ApplicationUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            EmailConfirmed = Input.EmailConfirmed,
            FaculteId = Input.FaculteId
        };

        var result = await _userManager.CreateAsync(user, Input.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }

        if (!string.IsNullOrEmpty(Input.Role))
        {
            await _userManager.AddToRoleAsync(user, Input.Role);
        }

        return RedirectToPage("Index");
    }

    private async Task LoadOptionsAsync()
    {
        RoleOptions = new List<SelectListItem>
        {
            new("Admin", "admin"),
            new("Faculty Admin", "facadmin"),
            new("Student", "student")
        };

        FaculteOptions = await _context.Facultes
            .OrderBy(f => f.Nom)
            .Select(f => new SelectListItem(f.Nom, f.Id.ToString()))
            .ToListAsync();
    }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; } = true;

        [Display(Name = "Role")]
        public string? Role { get; set; }

        [Display(Name = "Faculty")]
        public int? FaculteId { get; set; }
    }
}
