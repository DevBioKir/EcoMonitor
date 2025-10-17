using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using System.Text;
using EcoMonitor.App.Abstractions;
using EcoMonitor.Contracts.Contracts.Auth;
using EcoMonitor.Contracts.Contracts.Users;
using EcoMonitor.Core.Models.Auth;
using EcoMonitor.Core.Models.Users;
using EcoMonitor.Core.ValueObjects;
using EcoMonitor.DataAccess.Repositories.Auth;
using EcoMonitor.DataAccess.Repositories.Users;
using EcoMonitor.Infrastracture.Authentication;
using MapsterMapper;
using Microsoft.Extensions.Options;
using IPasswordHasher = EcoMonitor.Infrastracture.Abstractions.IPasswordHasher;

namespace EcoMonitor.App.Services.Authorization;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUserFactory _userFactory;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJWTService _jwtService;
    private readonly JwtSettings _jwtSettings;
    private readonly IMapper _mapper;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthService(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IUserFactory userFactory,
        IPasswordHasher passwordHasher,
        IJWTService jwtService,
        IOptions<JwtSettings> options,
        IMapper mapper,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _userFactory = userFactory;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _jwtSettings = options.Value;
        _mapper = mapper;
        _refreshTokenRepository = refreshTokenRepository;
    }
    
    public async Task<AuthResponse> LoginAsync(AuthRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null || !user.CheckPassword(request.Password, _passwordHasher))
            throw new InvalidOperationException("Invalid credentials");
        
        //user.UpdateLastLoggedAt(DateTime.UtcNow);
        
        // not tracked by context
        //update loggedAt
        await _userRepository.UpdateLastLoggedAtAsync(user, DateTime.UtcNow, cancellationToken);

        var accessToken = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        
        var refreshTokenHash = Hash(refreshToken);
        
        var refreshTokenDomain = RefreshToken.Create(
            user.Id,
            refreshTokenHash);
        
        await _refreshTokenRepository.AddRefreshTokenAsync(refreshTokenDomain);
        
        return new AuthResponse(
            accessToken,
            refreshToken,
            _jwtSettings.ExpiresInMinutes * 60);
    }
    
    private async Task<AuthResponse> RegisterUserAsync(
        RegisterUserRequest request, 
        Func<string, string, string, string, Guid, Core.Models.Users.User> createUserFunc,
        string roleName,
        CancellationToken cancellationToken = default)
    {
        var user =  await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user != null)
            throw new InvalidOperationException("User with this email already exists");

        var roleDomain = await _userRoleRepository.GetByNameASync(roleName, cancellationToken);

        var userDomain = createUserFunc(
            request.Firstname,
            request.Surname,
            request.Email,
            request.Password,
            roleDomain.Id);
        
        var userCreated = await _userRepository.AddAsync(userDomain, cancellationToken);
        
        var accessToken = _jwtService.GenerateToken(userCreated);
        var refreshToken = _jwtService.GenerateRefreshToken();
        
        var refreshTokenHash = Hash(refreshToken);
        
        var refreshTokenDomain = RefreshToken.Create(
            userCreated.Id,
            refreshTokenHash);
        
        await _refreshTokenRepository.AddRefreshTokenAsync(refreshTokenDomain);
        
        return new AuthResponse(accessToken, 
            refreshToken, 
            _jwtSettings.ExpiresInMinutes * 60);
    }

    public Task<AuthResponse> RegisterAsync(RegisterUserRequest request,
        CancellationToken cancellationToken = default)
        => RegisterUserAsync(request, _userFactory.Create, "User", cancellationToken);

    public Task<AuthResponse> RegisterAdminAsync(RegisterUserRequest request,
        CancellationToken cancellationToken = default)
        => RegisterUserAsync(request, _userFactory.Create, "Admin", cancellationToken);

    public Task<AuthResponse> RegisterManagerAsync(RegisterUserRequest request,
        CancellationToken cancellationToken = default)
        => RegisterUserAsync(request, _userFactory.Create, "Manager", cancellationToken);

    public async Task<AuthResponse> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        
        if (user == null)
            throw new KeyNotFoundException("User not found");
        if (!user.CheckPassword(currentPassword, _passwordHasher))
            throw new UnauthorizedAccessException("Current password is incorrect");
        
        //var newHash = PasswordHash.FromPlainPassword(newPassword, _passwordHasher);
        
        user.ChangePassword(currentPassword, newPassword,  _passwordHasher);
        await _userRepository.UpdateAsync(user, cancellationToken);
        
        var activeTokens = await _refreshTokenRepository
            .GetAllByUserIdAsync(userId, cancellationToken);
        
        foreach (var activeToken in activeTokens.Where(a => a.IsActive()))
        {
            await _refreshTokenRepository.RevokeAsync(activeToken, cancellationToken);
        }
        
        var accessToken = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        
        var refreshTokenHash = Hash(refreshToken);
        
        var refreshTokenDomain = RefreshToken.Create(
            user.Id,
            refreshTokenHash);
        
        await _refreshTokenRepository.AddRefreshTokenAsync(refreshTokenDomain);
        
        return new AuthResponse(
            accessToken,
            refreshToken,
            _jwtSettings.ExpiresInMinutes * 60);
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshTokenValue,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshTokenValue))
            throw new ArgumentException("Refresh token must be provided",  nameof(refreshTokenValue));
        
        var refreshTokenHash = Hash(refreshTokenValue);
        
        var storedRefreshToken = await _refreshTokenRepository.GetByTokenHashAsync(refreshTokenHash);
        if (storedRefreshToken == null || !storedRefreshToken.IsActive())
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        
        var user = await _userRepository.GetByIdAsync(storedRefreshToken.UserId, cancellationToken);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        await _refreshTokenRepository.RevokeAsync(storedRefreshToken, cancellationToken);
        
        var newAccessToken = _jwtService.GenerateToken(user);
        var newRefreshTokenValue =  _jwtService.GenerateRefreshToken();
        var newRefreshTokenHash = Hash(newRefreshTokenValue);
        
        var newRefreshToken = RefreshToken.Create(
            user.Id,
            newRefreshTokenHash);
        await _refreshTokenRepository.AddRefreshTokenAsync(newRefreshToken, cancellationToken);

        return new AuthResponse(
            newAccessToken,
            newRefreshTokenValue,
            _jwtSettings.ExpiresInMinutes * 60);
    }

    public async Task RevokeRefreshTokenAsync(string refreshTokenValue,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshTokenValue))
            throw new ArgumentException("Refresh token must be provided",  nameof(refreshTokenValue));
        
        var refreshTokenHash = Hash(refreshTokenValue);
        
        var storedRefreshToken = await _refreshTokenRepository.GetByTokenHashAsync(refreshTokenHash);
        if (storedRefreshToken == null || !storedRefreshToken.IsActive())
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        
        storedRefreshToken.Revoke();
        
        await _refreshTokenRepository.UpdateAsync(storedRefreshToken, cancellationToken);
    }

    private string Hash(string token)
    {
        using var sha = SHA256.Create();
        return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(token)));
    }
}