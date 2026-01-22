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
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<UserListItem> Users { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchEmail { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? FilterRole { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? FilterFaculteId { get; set; }

    public List<SelectListItem> RoleOptions { get; set; } = new();
    public List<SelectListItem> FaculteOptions { get; set; } = new();

    public async Task OnGetAsync()
    {
        var query = _context.Users
            .Include(u => u.Faculte)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(SearchEmail))
        {
            query = query.Where(u => u.Email != null && u.Email.Contains(SearchEmail));
        }

        if (FilterFaculteId.HasValue)
        {
            query = query.Where(u => u.FaculteId == FilterFaculteId);
        }

        var users = await query.OrderBy(u => u.Email).ToListAsync();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            
            if (!string.IsNullOrEmpty(FilterRole) && !roles.Contains(FilterRole))
                continue;

            Users.Add(new UserListItem
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                EmailConfirmed = user.EmailConfirmed,
                Roles = string.Join(", ", roles),
                FaculteName = user.Faculte?.Nom
            });
        }

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

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }
        return RedirectToPage();
    }

    public class UserListItem
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public string Roles { get; set; } = string.Empty;
        public string? FaculteName { get; set; }
    }
}
