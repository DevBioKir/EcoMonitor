namespace EcoMonitor.Contracts.Contracts.User
{
    public record UserRoleRequest(
        Guid Id,
        string Name,
        string Description,
        List<UserRequest> Users,
        List<PermissionRequest> Permissions);
}
