namespace EcoMonitor.Contracts.Contracts.User
{
    public record PermissionRequest(
        Guid Id,
        string Code,
        List<UserRoleRequest> Roles);
}
