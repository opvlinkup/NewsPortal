using Microsoft.AspNetCore.Identity;

namespace NewsPortal.Services;

public class IdentitySetupService(
    RoleManager<IdentityRole> roleManager,
    UserManager<IdentityUser> userManager,
    ILogger<IdentitySetupService> logger
) : IIdentitySetupService
{
    public async Task EnsureRolesAndUsersAsync()
    {
        var roles = new[] { "Admin", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role));
                if (result.Succeeded)
                {
                    logger.LogInformation("Role {Role} created successfully", role);
                }
                else
                {
                    logger.LogError(
                        "Failed to create role {Role}: {Errors}",
                        role,
                        string.Join(", ", result.Errors.Select(e => e.Description))
                    );
                }
            }
        }

        await EnsureUserAsync("Admin", "Admin123", "Admin");

        await EnsureUserAsync("User", "User123", "User");
    }

    private async Task EnsureUserAsync(string userName, string password, string role)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user == null)
        {
            user = new IdentityUser
            {
                UserName = userName,
                Email = $"{userName}@example.com",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                logger.LogError(
                    "Failed to create user {User}: {Errors}",
                    userName,
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );
                return;
            }
            logger.LogInformation("User {User} created successfully", userName);
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            var result = await userManager.AddToRoleAsync(user, role);
            if (result.Succeeded)
            {
                logger.LogInformation("User {User} added to role {Role}", userName, role);
            }
            else
            {
                logger.LogError(
                    "Failed to add user {User} to role {Role}: {Errors}",
                    userName,
                    role,
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );
            }
        }
    }
}
