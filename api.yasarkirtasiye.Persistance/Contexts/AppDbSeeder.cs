using api.yasarkirtasiye.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace api.yasarkirtasiye.Persistance.Contexts;

public static class AppDbSeeder
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        string[] roleNames = { "Admin", "Customer" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
        }

        var adminUser = await userManager.FindByEmailAsync("admin@yasarkirtasiye.com");

        if (adminUser == null)
        {
            var newAdmin = new User
            {
                UserName = "root",
                Email = "admin@yasarkirtasiye.com",
                FirstName = "Yaşar",
                LastName = "Kırtasiye",
                EmailConfirmed = true
            };

            var createPowerUser = await userManager.CreateAsync(newAdmin, "Password123!");
            if (createPowerUser.Succeeded)
            {
                // Here we assign the new user the "Admin" role
                await userManager.AddToRoleAsync(newAdmin, "Admin");
            }
        }
    }
}
