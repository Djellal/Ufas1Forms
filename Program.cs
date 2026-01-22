using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ufas1Forms.Data;
using Ufas1Forms.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<Ufas1Forms.Models.ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()  // Add roles support
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

var app = builder.Build();

// Seed roles, users, and sample data at startup
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await DbSeeder.SeedRolesAsync(serviceProvider);
    await DbSeeder.SeedFacultesAsync(serviceProvider);
    await DbSeeder.SeedDomainesAsync(serviceProvider);
    await DbSeeder.SeedAdminUserAsync(serviceProvider);
    await DbSeeder.SeedSampleUsersAsync(serviceProvider);
    await DbSeeder.SeedSampleFormsAsync(serviceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
