using EcoMonitor.Core.Models.Users;
using EcoMonitor.DataAccess.Entities.Users;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace EcoMonitor.DataAccess.Repositories.Users
{
    public class UserRepository
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

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var usersEntity = await _context.Users
                .Include(u => u.BinPhoto).ThenInclude(b => b.BinPhotoBinTypes)
                .Include(u => u.Role).ThenInclude(ur => ur.Permissions)
                .ToListAsync();

            return _mapper.Map<IEnumerable<User>>(usersEntity);
        }

        public async Task AddUserAsync(User user)
        {
            try
            {
                var userEntity = _mapper.Map<UserEntity>(user);
                await _context.Users.AddAsync(userEntity);
            }
            catch (Exception ex)
            {
                
            }
        }

        public async Task<IEnumerable<User>> GetUserIdAsync(Guid id)
        {
            var userEntity = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id)
                ;

            return _mapper.Map<IEnumerable<User>>(userEntity);
        }
    }
}
