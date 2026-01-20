using Microsoft.AspNetCore.Identity;
using TeamFlow.Web.Models;

namespace TeamFlow.Web.Data
{
    public static class IdentityAdminSeeder
    {
        public static async Task SeedAdminAsync(
            UserManager<ApplicationUser> userManager)
        {
            var email = Environment.GetEnvironmentVariable("TF_ADMIN_EMAIL");
            var password = Environment.GetEnvironmentVariable("TF_ADMIN_PASSWORD");

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    return;
                }
            }

            if (!await userManager.IsInRoleAsync(user, "Admin"))
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
