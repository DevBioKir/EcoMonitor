using EcoMonitor.App.Abstractions;
using EcoMonitor.Contracts.Contracts.Auth;
using EcoMonitor.Contracts.Contracts.Users;
using EcoMonitor.Core.ValueObjects;
using EcoMonitor.DataAccess.Repositories.Users;
using EcoMonitor.Infrastracture.Authentication;
using MapsterMapper;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Options;
using IPasswordHasher = EcoMonitor.Infrastracture.Abstractions.IPasswordHasher;

namespace EcoMonitor.App.Services.Authorization;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserFactory _userFactory;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJWTService _jwtService;
    private readonly JwtSettings _jwtSettings;
    private readonly IMapper _mapper;

    public AuthService(
        IUserRepository userRepository,
        IUserFactory userFactory, 
        IPasswordHasher passwordHasher,
        IJWTService jwtService,
        IOptions<JwtSettings> options,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _userFactory = userFactory;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _jwtSettings = options.Value;
        _mapper = mapper;
    }
    
    public async Task<AuthResponse> LoginAsync(AuthRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null || !user.CheckPassword(request.Password, _passwordHasher))
            return null;
        
        user.UpdateLastLoggedAt(DateTime.UtcNow);
        await _userRepository.UpdateLastLoggedAtAsync(user, DateTime.UtcNow, cancellationToken); //update loggedAt

        var accessToken = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();
        
        return new AuthResponse(
            accessToken,
            refreshToken,
            _jwtSettings.ExpiresInMinutes * 60);
    }

    public async Task<AuthResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        var existing =  await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing != null)
            throw new InvalidOperationException("User with this email already exists");
        
        var userDomain = _userFactory.Create(request.Firstname, request.Surname, request.Email, request.Password);
        await _userRepository.AddAsync(userDomain, cancellationToken);
        
        var accessToken = _jwtService.GenerateToken(userDomain);
        var refreshToken = _jwtService.GenerateRefreshToken();
        
        return new AuthResponse(accessToken, 
            refreshToken, 
            _jwtSettings.ExpiresInMinutes * 60);
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