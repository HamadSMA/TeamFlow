using Microsoft.AspNetCore.Identity;

namespace TeamFlow.Web.Data
{
    public static class IdentityRoleSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Employee" };

            foreach (var role in roles)
            {
                bool exists = await roleManager.RoleExistsAsync(role);
                if (!exists)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
