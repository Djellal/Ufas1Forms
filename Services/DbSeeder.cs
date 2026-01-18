using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ufas1Forms.Services
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Define the roles to be seeded
            string[] roles = { "admin", "facadmin", "student" };

            foreach (var roleName in roles)
            {
                // Check if the role already exists
                var roleExists = await roleManager.RoleExistsAsync(roleName);

                if (!roleExists)
                {
                    // Create the role
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Define admin user details
            string adminEmail = "djellal@univ-setif.dz";
            string adminPassword = "DhB@571982";
            string adminRole = "admin";

            // Check if admin user already exists
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                // Create the admin user
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true // Confirm the email by default
                };

                var createUserResult = await userManager.CreateAsync(adminUser, adminPassword);

                if (createUserResult.Succeeded)
                {
                    // Assign the admin user to the admin role
                    await userManager.AddToRoleAsync(adminUser, adminRole);
                }
            }
            else
            {
                // If user exists, ensure they have the admin role
                bool isInRole = await userManager.IsInRoleAsync(adminUser, adminRole);
                if (!isInRole)
                {
                    await userManager.AddToRoleAsync(adminUser, adminRole);
                }
            }
        }
    }
}