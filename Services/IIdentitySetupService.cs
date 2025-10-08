namespace NewsPortal.Services;

public interface IIdentitySetupService
{
    Task EnsureRolesAsync();
}
