using EcoMonitor.Core.Models.Users;
using EcoMonitor.DataAccess.Entities.Users;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace EcoMonitor.DataAccess.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly EcoMonitorDbContext _context;
        private readonly IMapper _mapper;

        public UserRepository(
            EcoMonitorDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _context.Users
                .Include(u => u.Role)
                .ThenInclude(r => r.Permissions)
                .Include(u => u.BinPhoto)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<User>>(entities);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            var entity = _mapper.Map<UserEntity>(user);
            await _context.Users.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Users
                .Include(u => u.Role)
                .ThenInclude(r => r.Permissions)
                .Include(u => u.BinPhoto)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

            return _mapper.Map<User>(entity);
        }

        public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Users
                .Include(u => u.Role)
                .ThenInclude(r => r.Permissions)
                .Include(u => u.BinPhoto)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            return _mapper.Map<User>(entity);
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);
            if (entity == null) return;


            user.Adapt(entity);

            _context.Users.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            if (entity != null)
            {
                _context.Users.Remove(entity);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
