using Lab2_2.Models;
using Microsoft.AspNetCore.Identity;

namespace Lab2_2
{
    public static class Extensions
    {
        public static async Task<RoleManager<IdentityRole>> EnsureRolesCreated(this RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = ["Admin", "User", "Supervisor", "PasswordValid"];
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            return roleManager;
        }
        public static async Task<UserManager> EnsureAdminCreated(this UserManager userManager)
        {
            var adminUser = await userManager.FindByNameAsync("ADMIN");
            if (adminUser is not null) return userManager;

            //Create admin if not present
            adminUser = new User
            {
                UserName = "ADMIN",
                Email = "admin@admin.com",
                EmailConfirmed = true,

            };

            const string password = "P@$$w0rd";

            var result = await userManager.CreateAsync(adminUser, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                await userManager.AddToRoleAsync(adminUser, "Supervisor");
                await userManager.AddToRoleAsync(adminUser, "PasswordValid");
                Console.WriteLine("Admin created!");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error.Description);
                }
            }
            return userManager;
        }
    }
}
