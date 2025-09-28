using EcoMonitor.App.Abstractions;
using EcoMonitor.App.Services.Authorization;
using EcoMonitor.Contracts.Contracts.User;
using EcoMonitor.Core.ValueObjects;
using EcoMonitor.DataAccess.Repositories.Users;
using EcoMonitor.Infrastracture.Authentication;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace EcoMonitor.App.Services.User;

public class UserService : IUserService
{
    private readonly IUserFactory _userFactory;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IJWTService _jwtTokenService;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserFactory userFactory,
        IMapper mapper,
        IUserRepository userRepository,
        IAuthService authService,
        IAuthorizationService authorizationService,
        IJWTService jwtTokenService,
        ILogger<UserService> logger)
    {
        _userFactory = userFactory;
        _mapper = mapper;
        _userRepository = userRepository;
        _authService = authService;
        _authorizationService = authorizationService;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }


    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(
        Guid currentUserId, 
        CancellationToken cancellationToken = default)
    {
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);
        if (currentUser == null) 
            throw new UnauthorizedAccessException("Current user not found");
        
        if (!currentUser.HasPermission(Permission.UsersView))
            throw new UnauthorizedAccessException($"You do not have permission to view all users");
        
        var users =  await _userRepository.GetAllAsync(cancellationToken);
        
        return _mapper.Map<IReadOnlyList<UserResponse>>(users);
    }

    public async Task AddAsync(UserRequest user, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);
        if (currentUser == null) 
            throw new UnauthorizedAccessException("Current user not found");
        
        if (!currentUser.HasPermission(Permission.UsersAdd))
            throw new UnauthorizedAccessException("You do not have permission to add users");
        
        var userDomain = _userFactory.Create(
                                    user.Firstname,
                                    user.Surname,
                                    user.Email,
                                    user.Password);
        
        await _userRepository.AddAsync(userDomain, cancellationToken);
    }

    public async Task<UserResponse> GetByIdAsync(Guid id, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);
        if (currentUser == null) 
            throw new UnauthorizedAccessException("Current user not found");
        
        if (!currentUser.HasPermission(Permission.UsersView))
            throw new UnauthorizedAccessException($"You do not have permission to view all users");
        
        var user =  await _userRepository.GetByIdAsync(id, cancellationToken);
        
        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var userEmail =  await _userRepository.GetByEmailAsync(email, cancellationToken);
        
        return _mapper.Map<UserResponse>(userEmail);
    }

    public async Task<UserResponse> UpdateAsync(UserRequest user, CancellationToken cancellationToken = default)
    {
        var selectedUser = await _userRepository.GetByIdAsync(user.Id, cancellationToken);
        if (selectedUser == null)
            throw new KeyNotFoundException($"User with id {user.Id} not found");
 
        var userDomain = _mapper.Map<Core.Models.Users.User>(user);
        await _userRepository.UpdateAsync(userDomain, cancellationToken);
        
        return _mapper.Map<UserResponse>(user);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _userRepository.DeleteAsync(id, cancellationToken);
    }
}
