namespace EcoMonitor.Contracts.Contracts.User
{
    public record UserRoleResponse(
        Guid Id,
        string Name,
        string Description,
        //List<UserWithPhotosResponse> Users,
        List<PermissionResponse> Permissions
        );
}