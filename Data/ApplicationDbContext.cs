using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ufas1Forms.Models;

namespace Ufas1Forms.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Etablissement> Etablissements { get; set; }
    public DbSet<Faculte> Facultes { get; set; }
    public DbSet<Domaine> Domaines { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

       
    }
}
