namespace EcoMonitor.Contracts.Contracts.User
{
    public record PermissionResponse(
        Guid Id,
        string Code,
        List<UserRoleResponse> Roles);
}