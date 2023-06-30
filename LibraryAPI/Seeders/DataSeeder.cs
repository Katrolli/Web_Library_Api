using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using LibraryAPI.Models;

namespace LibraryAPI.Seeders
{
    public static class SeedData
    {
        public static async Task Initialize(UserManager<User> userManager, RoleManager<Role> roleManager, ITokenService tokenService)
        {
            await SeedRoles(roleManager);
            await SeedAdminUser(userManager, tokenService);
        }

        private static async Task SeedRoles(RoleManager<Role> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                var adminRole = new Role { Name = "Admin" };
                await roleManager.CreateAsync(adminRole);
            }

            if (!await roleManager.RoleExistsAsync("Author"))
            {
                var authorRole = new Role { Name = "Author" };
                await roleManager.CreateAsync(authorRole);
            }
        }

        private static async Task SeedAdminUser(UserManager<User> userManager, ITokenService tokenService)
        {
            var adminEmail = "admin@example.com";
            var adminPassword = "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            // Create admin user if it doesn't exist
            if (adminUser == null)
            {
                adminUser = new User { Name = "Admin", Email = adminEmail, UserName = adminEmail, RoleId = 1 };
                var refreshToken = tokenService.GenerateRefreshToken();
                adminUser.RefreshToken = refreshToken;
                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                    await userManager.UpdateAsync(adminUser);
                }
                else
                {
                    var errors = result.Errors.Select(e => e.Description);
                    Console.WriteLine("Failed to create admin user. Errors: " + string.Join(", ", errors));
                }
            }
        }
    }
}
