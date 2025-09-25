using EcoMonitor.Contracts.Contracts.User;
using EcoMonitor.DataAccess.Repositories.Users;
using EcoMonitor.Infrastracture.Authentication;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace EcoMonitor.App.Services.User;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly  IJWTService _jwtTokenService;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IMapper mapper, 
        IUserRepository userRepository, 
        IJWTService jwtTokenService, 
        ILogger<UserService> logger)
    {
        _mapper = mapper;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }


    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users =  await _userRepository.GetAllAsync(cancellationToken);
        
        return _mapper.Map<IReadOnlyList<UserResponse>>(users);
    }

    public async Task AddAsync(UserRequest user, CancellationToken cancellationToken = default)
    {
        var userDomain = _mapper.Map<Core.Models.Users.User>(user);
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

    public async Task<UserResponse> UpdateAsync(UserRequest user, CancellationToken cancellationToken = default)
    {
        var userDomain = _mapper.Map<Core.Models.Users.User>(user);
        await _userRepository.UpdateAsync(userDomain, cancellationToken);
        
        return _mapper.Map<UserResponse>(user);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _userRepository.DeleteAsync(id, cancellationToken);
    }
}
