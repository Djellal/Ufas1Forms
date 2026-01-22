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
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public EditModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<SelectListItem> RoleOptions { get; set; } = new();
    public List<SelectListItem> FaculteOptions { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        Input = new InputModel
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            EmailConfirmed = user.EmailConfirmed,
            Role = roles.FirstOrDefault(),
            FaculteId = user.FaculteId
        };

        await LoadOptionsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadOptionsAsync();

        if (!ModelState.IsValid)
            return Page();

        var user = await _userManager.FindByIdAsync(Input.Id);
        if (user == null)
            return NotFound();

        user.Email = Input.Email;
        user.UserName = Input.Email;
        user.EmailConfirmed = Input.EmailConfirmed;
        user.FaculteId = Input.FaculteId;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }

        // Update password if provided
        if (!string.IsNullOrWhiteSpace(Input.NewPassword))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResult = await _userManager.ResetPasswordAsync(user, token, Input.NewPassword);
            if (!passwordResult.Succeeded)
            {
                foreach (var error in passwordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
        }

        // Update role
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        
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
        public string Id { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmNewPassword { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "Role")]
        public string? Role { get; set; }

        [Display(Name = "Faculty")]
        public int? FaculteId { get; set; }
    }
}
