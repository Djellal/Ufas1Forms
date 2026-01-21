using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Ufas1Forms.Models;

public class ApplicationUser : IdentityUser
{
    public int? FaculteId { get; set; }

    [ForeignKey(nameof(FaculteId))]
    public Faculte? Faculte { get; set; }
}
