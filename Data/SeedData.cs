using CollectionManagement.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace CollectionManagement.Data
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<User> userManager)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

            string[] roleNames = { "Admin", "User", "NonUser" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new Role { Name = roleName });
                }
            }

            // Create a default admin user if none exists
            if (userManager.FindByEmailAsync("admin@example.com").Result == null)
            {
                User user = new User
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    SelectedLanguage = "en",
                    SelectedTheme = "light"
                };

                IdentityResult result = await userManager.CreateAsync(user, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
