using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using System.Text;
using EcoMonitor.App.Abstractions;
using EcoMonitor.Contracts.Contracts.Auth;
using EcoMonitor.Contracts.Contracts.Users;
using EcoMonitor.Core.Models.Auth;
using EcoMonitor.DataAccess.Repositories.Auth;
using EcoMonitor.DataAccess.Repositories.Users;
using EcoMonitor.Infrastracture.Authentication;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using IPasswordHasher = EcoMonitor.Infrastracture.Abstractions.IPasswordHasher;

namespace EcoMonitor.App.Services.Authorization;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUserRegisterFactoryResolver _factoryResolver;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJWTService _jwtService;
    private readonly JwtSettings _jwtSettings;
    private readonly IMapper _mapper;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        
        IUserRegisterFactory userRegisterFactory,
        IUserRegisterFactory managerRegisterFactory,
        IUserRegisterFactoryResolver factoryResolver,
        
        IPasswordHasher passwordHasher,
        IJWTService jwtService,
        IOptions<JwtSettings> options,
        IMapper mapper,
        IRefreshTokenRepository refreshTokenRepository,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _factoryResolver = factoryResolver;
        
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _jwtSettings = options.Value;
        _mapper = mapper;
        _refreshTokenRepository = refreshTokenRepository;
        _logger = logger;
    }
    
    public async Task<AuthResponse> LoginAsync(AuthRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null || !user.CheckPassword(request.Password, _passwordHasher))
            throw new UnauthorizedAccessException("Неверный логин или пароль");
        
        //user.UpdateLastLoggedAt(DateTime.UtcNow);
        
        // not tracked by context
        //update loggedAt
        await _userRepository.UpdateLastLoggedAtAsync(user, DateTime.UtcNow, cancellationToken);
        
        var activeTokens = await _refreshTokenRepository.GetAllByUserIdAsync(user.Id, cancellationToken);
        foreach (var token in activeTokens)
        {
            await _refreshTokenRepository.RevokeAsync(token, cancellationToken);
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
    
    public async Task<AuthResponse> RegisterAsync(
        RegisterUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var user =  await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user != null)
            throw new InvalidOperationException("User with this email already exists");
        
        var userRole = await _userRoleRepository.GetRoleByIdAsNoTrackingAsync(request.RoleName, cancellationToken);
        
        // Select a factory by role name
        var factory = _factoryResolver.Resolve(userRole.Name);
        
        if (userRole is null)
            throw new InvalidOperationException("Role not found");

        //var roleDomain = await _userRoleRepository.GetByNameASync(roleName, cancellationToken);

        var userDomain = factory.CreateUser(
            request.Firstname,
            request.Surname,
            request.Email,
            request.Password);
        
        var userCreated = await _userRepository.AddAsync(userDomain, cancellationToken);
        
        var accessToken = _jwtService.GenerateToken(userCreated);
        var refreshToken = _jwtService.GenerateRefreshToken();
        
        var refreshTokenHash = Hash(refreshToken);
        
        var refreshTokenDomain = RefreshToken.Create(
            userCreated.Id,
            refreshTokenHash);
        
        await _refreshTokenRepository.AddRefreshTokenAsync(refreshTokenDomain, cancellationToken);
        
        return new AuthResponse(accessToken, 
            refreshToken, 
            _jwtSettings.ExpiresInMinutes * 60);
    }

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
        // if (storedRefreshToken == null || !storedRefreshToken.IsActive())
        //     throw new UnauthorizedAccessException("Invalid or expired refresh token");
        
        if (storedRefreshToken == null || !storedRefreshToken.IsActive())
        {
            _logger.LogWarning("Attempted to revoke non-existent or inactive token");
            return;
        }
        
        storedRefreshToken.Revoke();
        
        await _refreshTokenRepository.UpdateAsync(storedRefreshToken, cancellationToken);
    }

    private string Hash(string token)
    {
        using var sha = SHA256.Create();
        return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(token)));
    }

    public async Task BlockUserAsync(
        Guid id, 
        string reason,
        TimeSpan duration,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("User ID is required");

        if (string.IsNullOrEmpty(reason))
            throw new ArgumentException("Reason is required");
        
        _logger.LogInformation("Blocking user {UserId} for reason: {Reason}", id, reason);
        
        try
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            
            if (user == null)
                throw new NotFoundException($"User {id} not found");
            
            user.BlockUser(reason, duration);
            
            await _userRepository.UpdateAsync(user, cancellationToken);
            _logger.LogInformation("User {UserId} successfully blocked", id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task UnlockUserAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("User ID is required");
        
        _logger.LogInformation("Unlocking user {UserId}", id);
        
        try
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            
            if (user == null)
                throw new NotFoundException($"User {id} not found");
            
            user.UnlockAccount();
            
            await _userRepository.UpdateAsync(user, cancellationToken);
            _logger.LogInformation("User {UserId} successfully unlocked", id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}