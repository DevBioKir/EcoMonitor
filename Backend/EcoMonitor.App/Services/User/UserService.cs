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
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserFactory userFactory,
        IMapper mapper,
        IUserRepository userRepository,
        IAuthorizationService authorizationService,
        ILogger<UserService> logger)
    {
        _userFactory = userFactory;
        _mapper = mapper;
        _userRepository = userRepository;
        _authorizationService = authorizationService;
        _logger = logger;
    }
    
    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(
        Guid currentUserId, 
        CancellationToken cancellationToken = default)
    {
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken) ??
            throw new UnauthorizedAccessException("Current user not found");
        
        _authorizationService.CheckPermisson(currentUser, Permission.UsersView);
        
        var users =  await _userRepository.GetAllAsync(cancellationToken);
        
        return _mapper.Map<IReadOnlyList<UserResponse>>(users);
    }

    public async Task AddAsync(UserRequest user, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken) ??
            throw new UnauthorizedAccessException("Current user not found");
        
        _authorizationService.CheckPermisson(currentUser, Permission.UsersAdd);
        
        var userDomain = _userFactory.Create(
                                    user.Firstname,
                                    user.Surname,
                                    user.Email,
                                    user.Password);
        
        await _userRepository.AddAsync(userDomain, cancellationToken);
    }

    public async Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user =  await _userRepository.GetByIdAsync(id, cancellationToken);
        
        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var userEmail =  await _userRepository.GetByEmailAsync(email, cancellationToken);
        
        return _mapper.Map<UserResponse>(userEmail);
    }

    public async Task<UserResponse> UpdateAsync(UserRequest user, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken) ??
            throw new UnauthorizedAccessException("Current user not found");
        
        _authorizationService.CheckPermisson(currentUser, Permission.UsersEdit);
        
        var selectedUser = await _userRepository.GetByIdAsync(user.Id, cancellationToken) ??
            throw new KeyNotFoundException($"User with id {user.Id} not found");
        
        selectedUser.UpdateFirstname(user.Firstname);
        selectedUser.UpdateSurname(user.Surname);
        //selectedUser.UpdateEmail(user.Email);

        await _userRepository.UpdateAsync(selectedUser, cancellationToken);
        
        return _mapper.Map<UserResponse>(user);
    }

    public async Task DeleteAsync(Guid id, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken) ??
            throw new UnauthorizedAccessException("Current user not found");
        
        _authorizationService.CheckPermisson(currentUser, Permission.UsersDelete);
        
        await _userRepository.DeleteAsync(id, cancellationToken);
    }
}
