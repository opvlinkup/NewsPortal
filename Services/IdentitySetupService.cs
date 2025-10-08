using Microsoft.AspNetCore.Identity;

namespace NewsPortal.Services;

public class IdentitySetupService(RoleManager<IdentityRole> roleManager) : IIdentitySetupService
{
    public async Task EnsureRolesAsync()
    {
        const string adminRole = "Admin";

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }
        
    }
}
