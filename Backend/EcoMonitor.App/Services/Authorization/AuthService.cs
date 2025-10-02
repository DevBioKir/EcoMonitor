using EcoMonitor.App.Abstractions;
using EcoMonitor.Contracts.Contracts.Auth;
using EcoMonitor.Contracts.Contracts.Users;
using EcoMonitor.Core.ValueObjects;
using EcoMonitor.DataAccess.Repositories.Users;
using EcoMonitor.Infrastracture.Authentication;
using MapsterMapper;
using Microsoft.AspNet.Identity;
using IPasswordHasher = EcoMonitor.Infrastracture.Abstractions.IPasswordHasher;

namespace EcoMonitor.App.Services.Authorization;

public class AuthService(IUserRepository _userRepository,
    IUserFactory _userFactory,
    IPasswordHasher _passwordHasher, 
    IJWTService _jwtService, 
    JwtSettings _jwtSettings,
    IMapper _mapper) : IAuthService
{
    public async Task<AuthResponse> LoginAsync(AuthRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null || !user.CheckPassword(request.Password, _passwordHasher))
            return null;
        
        user.UpdateLastLoggedAt(DateTime.UtcNow);
        await _userRepository.UpdateAsync(user, cancellationToken); //update loggedAt

        var token = _jwtService.GenerateToken(user);
        return new AuthResponse(
            token,
            _jwtSettings.ExpiresInMinutes * 60);
    }

    public async Task<AuthResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        var existing =  await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing == null)
            throw new InvalidOperationException("User with this email already exists");
        
        var userDomain = _userFactory.Create(request.Firstname, request.Surname, request.Email, request.Password);
        await _userRepository.AddAsync(userDomain, cancellationToken);
        var token = _jwtService.GenerateToken(userDomain);
        
        return new AuthResponse(token, _jwtSettings.ExpiresInMinutes * 60);
    }

    public async Task ChangePassword(Guid userId, string currentPassword, string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        
        if (user == null) 
            throw new KeyNotFoundException("User not found");
        if (!user.CheckPassword(currentPassword, _passwordHasher))
            throw new UnauthorizedAccessException("Current password is incorrect");
        
        var newHash = PasswordHash.FromPlainPassword(newPassword, _passwordHasher);
    }
}