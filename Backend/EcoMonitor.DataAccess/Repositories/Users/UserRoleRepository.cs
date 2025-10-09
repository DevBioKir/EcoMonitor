using EcoMonitor.Core.Models.Users;
using EcoMonitor.DataAccess.Entities.Users;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace EcoMonitor.DataAccess.Repositories.Users;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly EcoMonitorDbContext _context;
    private readonly IMapper _mapper;
    
    public UserRoleRepository(
        EcoMonitorDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<UserRole> GetByNameASync(string name, CancellationToken cancellationToken = default)
    {
        var userRoleEntity = await _context.UserRoles.FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
        return _mapper.Map<UserRole>(userRoleEntity);
    }

    public async Task<UserRole> GetByIdASync(Guid id, CancellationToken cancellationToken = default)
    {
        // If the entity is already loaded and tracked by EF Core in the current context, it is returned from memory - the database query is not executed.
        // If not, it queries the database and searches for the key.
        // If the entity is found, it attaches it to the context and returns it.
        // If not found, it returns null.
        // It uses an array of keys (in this case, a single id key), since an entity can have a composite key.
        var userRoleEntity = await _context.UserRoles.FindAsync(new object[] { id }, cancellationToken);
        return _mapper.Map<UserRole>(userRoleEntity);
    }
}