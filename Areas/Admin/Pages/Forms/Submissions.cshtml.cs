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
public class SubmissionsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public SubmissionsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public Form Form { get; set; } = default!;
    public List<SubmissionListItem> Submissions { get; set; } = new();
    public int TotalCount { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? FilterFaculteId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? FilterDomaineId { get; set; }

    public List<SelectListItem> FaculteOptions { get; set; } = new();
    public List<SelectListItem> DomaineOptions { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var isAdmin = User.IsInRole("admin");

        var formQuery = _context.Forms.AsNoTracking();

        if (!isAdmin && currentUser?.FaculteId != null)
        {
            formQuery = formQuery.Where(f => f.FaculteId == currentUser.FaculteId);
        }

        var form = await formQuery.FirstOrDefaultAsync(f => f.Id == id);

        if (form == null)
        {
            return NotFound();
        }

        Form = form;

        var submissionsQuery = _context.FormSubmissions
            .Where(s => s.FormId == id)
            .AsQueryable();

        if (!isAdmin && currentUser?.FaculteId != null)
        {
            submissionsQuery = submissionsQuery.Where(s => s.FaculteId == currentUser.FaculteId);
            FilterFaculteId = currentUser.FaculteId;
        }
        else if (FilterFaculteId.HasValue)
        {
            submissionsQuery = submissionsQuery.Where(s => s.FaculteId == FilterFaculteId);
        }

        if (FilterDomaineId.HasValue)
        {
            submissionsQuery = submissionsQuery.Where(s => s.DomaineId == FilterDomaineId);
        }

        Submissions = await submissionsQuery
            .Include(s => s.SubmittedByUser)
            .Include(s => s.Faculte)
            .Include(s => s.Domaine)
            .OrderByDescending(s => s.SubmittedAt)
            .Select(s => new SubmissionListItem
            {
                Id = s.Id,
                SubmittedAt = s.SubmittedAt,
                SubmittedByEmail = s.SubmittedByUser != null ? s.SubmittedByUser.Email : null,
                Status = s.Status,
                FaculteName = s.Faculte != null ? s.Faculte.Nom : null,
                DomaineName = s.Domaine != null ? s.Domaine.Nom : null
            })
            .ToListAsync();

        TotalCount = Submissions.Count;

        if (isAdmin)
        {
            FaculteOptions = await _context.Facultes
                .OrderBy(f => f.Nom)
                .Select(f => new SelectListItem { Value = f.Id.ToString(), Text = f.Nom })
                .ToListAsync();
        }

        if (FilterFaculteId.HasValue)
        {
            DomaineOptions = await _context.Domaines
                .Where(d => d.FaculteId == FilterFaculteId)
                .OrderBy(d => d.Nom)
                .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Nom })
                .ToListAsync();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id, int submissionId)
    {
        var submission = await _context.FormSubmissions.FindAsync(submissionId);
        if (submission != null)
        {
            _context.FormSubmissions.Remove(submission);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage(new { id });
    }

    public class SubmissionListItem
    {
        public int Id { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string? SubmittedByEmail { get; set; }
        public SubmissionStatus Status { get; set; }
        public string? FaculteName { get; set; }
        public string? DomaineName { get; set; }
    }
}
