using Microsoft.AspNetCore.Identity;
using Sem_Fashion_Kids_Clothing_Website.Models;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // =========================
        // ROLES
        // =========================
        string[] roles = { "Admin", "Customer" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // =========================
        // ADMIN USER
        // =========================
        string adminEmail = "admin@semfashion.com";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var user = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Admin",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, "Admin@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
        else
        {
            // =========================
            // FIX EXISTING ADMIN FULLNAME
            // =========================
            if (string.IsNullOrWhiteSpace(adminUser.FullName))
            {
                adminUser.FullName = "System Admin";
                await userManager.UpdateAsync(adminUser);
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // =========================
        // CUSTOMER USER
        // =========================
        string customerEmail = "customer@semfashion.com";

        var customerUser = await userManager.FindByEmailAsync(customerEmail);

        if (customerUser == null)
        {
            var user = new ApplicationUser
            {
                UserName = customerEmail,
                Email = customerEmail,
                FullName = "Default Customer",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, "Customer@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Customer");
            }
        }
        else
        {
            // =========================
            // FIX EXISTING CUSTOMER FULLNAME
            // =========================
            if (string.IsNullOrWhiteSpace(customerUser.FullName))
            {
                customerUser.FullName = "Default Customer";
                await userManager.UpdateAsync(customerUser);
            }

            if (!await userManager.IsInRoleAsync(customerUser, "Customer"))
            {
                await userManager.AddToRoleAsync(customerUser, "Customer");
            }
        }
    }
}