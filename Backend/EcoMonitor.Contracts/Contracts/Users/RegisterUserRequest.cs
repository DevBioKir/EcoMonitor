namespace EcoMonitor.Contracts.Contracts.Users
{
    public record RegisterUserRequest( 
        string Firstname,
        string Surname,
        string Email,
        string Password,
        Guid RoleName);
}
