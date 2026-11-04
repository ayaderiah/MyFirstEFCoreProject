
using Microsoft.AspNetCore.Identity;

namespace CustomerManagement.Data
{
    public class RoleSeeder
    {
        public static async Task SeedRolesAsync(
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            var roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            var userManager = serviceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            // Create the three roles
            string[] roles = { "Admin", "SubAdmin", "User" };

            foreach (string role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(
                        new IdentityRole(role));

                    if (!result.Succeeded)
                    {
                        throw new Exception(
                            "Failed to create role: " + role);
                    }
                }
            }

           
            var testUser = await userManager.FindByEmailAsync(
                "testuser@example.com");

            if (testUser != null)
            {
                await AssignRoleAsync(userManager, testUser, "User");
            }

            // Create Admin *-*
            await CreateTestUserAsync(
                userManager,
                configuration,
                "admin@test.local",
                "SeedUsers:AdminPassword",
                "Admin");

            // Create Manager 1 
            await CreateTestUserAsync(
                userManager,
                configuration,
                "manager1@test.local",
                "SeedUsers:Manager1Password",
                "SubAdmin");

            // Create Manager 2
            await CreateTestUserAsync(
                userManager,
                configuration,
                "manager2@test.local",
                "SeedUsers:Manager2Password",
                "SubAdmin");
        }

      
        private static async Task CreateTestUserAsync(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            string email,
            string passwordKey,
            string role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                string? password = configuration[passwordKey];

                if (string.IsNullOrWhiteSpace(password))
                {
                    throw new Exception(
                        "Missing test password: " + passwordKey);
                }

                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(
                    user, password);

                if (!result.Succeeded)
                {
                    throw new Exception(
                        "Failed to create test account: " + email
                        + " - " + string.Join(", ",
                            result.Errors.Select(e => e.Description)));
                }
            }

           
            var currentRoles = await userManager.GetRolesAsync(user);

            if (currentRoles.Count > 0 &&
                !currentRoles.Contains(role))
            {
                throw new Exception(
                    "Existing account has a different role: " + email);
            }

            await AssignRoleAsync(userManager, user, role);
        }

      
        private static async Task AssignRoleAsync(
            UserManager<ApplicationUser> userManager,
            ApplicationUser user,
            string role)
        {
            if (!await userManager.IsInRoleAsync(user, role))
            {
                var result = await userManager.AddToRoleAsync(
                    user, role);

                if (!result.Succeeded)
                {
                    throw new Exception(
                        "Failed to assign role: " + role);
                }
            }
        }
    }
}
