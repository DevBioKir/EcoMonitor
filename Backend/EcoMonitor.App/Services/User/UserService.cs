using EcoMonitor.App.Abstractions;
using EcoMonitor.App.Services.Authorization;
using EcoMonitor.Contracts.Contracts.User;
using EcoMonitor.Contracts.Contracts.Users;
using EcoMonitor.Contracts.Contracts.Users.UpdateUser;
using EcoMonitor.Core.ValueObjects;
using EcoMonitor.DataAccess.Repositories.Users;
using EcoMonitor.Infrastracture.Authentication;
using MapsterMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace EcoMonitor.App.Services.User;

public class UserService : IUserService
{
    private readonly IUserFactory _userFactory;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserFactory userFactory,
        IMapper mapper,
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IAuthorizationService authorizationService,
        ILogger<UserService> logger)
    {
        _userFactory = userFactory;
        _mapper = mapper;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _authorizationService = authorizationService;
        _logger = logger;
    }
    
    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(Guid? currentUserId,
        CancellationToken cancellationToken = default)
    {
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken) ??
            throw new UnauthorizedAccessException("Current user not found");
        
        _authorizationService.CheckPermisson(currentUser, Permission.UsersView);
        
        var users =  await _userRepository.GetAllAsync(cancellationToken);
        
        return _mapper.Map<IReadOnlyList<UserResponse>>(users);
    }

    public async Task AddAsync(UserRequest user, Guid? currentUserId, CancellationToken cancellationToken = default)
    {
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken) ??
            throw new UnauthorizedAccessException("Current user not found");
        
        _authorizationService.CheckPermisson(currentUser, Permission.UsersAdd);
        
        var role = await _userRoleRepository.GetByNameASync("User", cancellationToken);
        if (role == null)
        {
            throw new InvalidOperationException($"Role {role.Name} not found");
        }
        
        var userDomain = _userFactory.Create(
            user.Firstname, 
            user.Surname,
            user.Email,
            user.Password,
            role.Id);
        
        await _userRepository.AddAsync(userDomain, cancellationToken);
    }

    public async Task<UserWithPhotosResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user =  await _userRepository.GetByIdAsync(id, cancellationToken);
        
        return _mapper.Map<UserWithPhotosResponse>(user);
    }

    public async Task<UserWithPhotosResponse> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var userEmail =  await _userRepository.GetByEmailAsync(email, cancellationToken);
        
        return _mapper.Map<UserWithPhotosResponse>(userEmail);
    }

    public async Task<UserResponse> UpdateAsync(
        Guid actorId, Guid userId, UpdateUserDTO request, CancellationToken cancellationToken = default)
    {
        var actor = await _userRepository.GetByIdAsync(actorId, cancellationToken) ?? 
                      throw new UnauthorizedAccessException("Actor not found");
        
        _authorizationService.CheckPermisson(actor, Permission.UsersEdit);
        
        var selectedUser = await _userRepository.GetByIdAsync(userId, cancellationToken) ?? 
                           throw new KeyNotFoundException($"User with id {userId} not found");
        
        selectedUser.UpdatePersonalInfo(request.Firstname, request.Surname);
        selectedUser.UpdateEmail(request.Email);
        selectedUser.ChangeRole(request.UserRole);
        
        await _userRepository.UpdateAsync(selectedUser, cancellationToken);
        
        return _mapper.Map<UserResponse>(selectedUser);
    }
    
    // public async Task<UserWithPhotosResponse> UpdateAsync(UserRequest user, Guid? currentUserId,
    //     CancellationToken cancellationToken = default)
    // {
    //     var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken) ??
    //         throw new UnauthorizedAccessException("Current user not found");
    //     
    //     _authorizationService.CheckPermisson(currentUser, Permission.UsersEdit);
    //     
    //     var selectedUser = await _userRepository.GetByIdAsync(user.Id, cancellationToken) ??
    //         throw new KeyNotFoundException($"User with id {user.Id} not found");
    //     
    //     // if (selectedUser.RowVersion != user.RowVersion)
    //     //     throw new DbUpdateConcurrencyException("User was modified by another user");
    //     
    //     selectedUser.UpdateProfile(user.Firstname, user.Surname);
    //     //selectedUser.UpdateEmail(user.Email);
    //
    //     await _userRepository.UpdateAsync(selectedUser, cancellationToken);
    //     
    //     return _mapper.Map<UserWithPhotosResponse>(selectedUser);
    // }
    
    
    
    // public async Task DeleteAsync(Guid id, Guid? currentUserId, CancellationToken cancellationToken = default)
    // {
    //     var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken) ??
    //         throw new UnauthorizedAccessException("Current user not found");
    //     
    //     _authorizationService.CheckPermisson(currentUser, Permission.UsersDelete);
    //     
    //     await _userRepository.DeleteAsync(id, cancellationToken);
    // }
}
