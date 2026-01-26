using EcoMonitor.Contracts.Contracts.Users.UpdateUser;
using EcoMonitor.Core.Models.Users;
using EcoMonitor.DataAccess.Entities.Users;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace EcoMonitor.DataAccess.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly EcoMonitorDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(
            EcoMonitorDbContext context,
            IMapper mapper, ILogger<UserRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _context.Users
                .Include(u => u.Role)
                .ThenInclude(r => r.Permissions)
                //.Include(u => u.BinPhoto)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<User>>(entities);
        }

        public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
        {
            var roleEntity = _context.UserRoles.Local.FirstOrDefault(r => r.Id == user.RoleId);
            if (roleEntity == null)
            {
                roleEntity = await _context.UserRoles.FindAsync(user.RoleId);
                if (roleEntity == null)
                    throw new InvalidOperationException("Role not found");
            }
            
            var entity = _mapper.Map<UserEntity>(user);
            // Attaching a role to the current context
            // _context.UserRoles.Attach(roleEntity);
            // Linking a user to a role
            entity.RoleId = roleEntity.Id;
            
            await _context.Users.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            
            var updatedUser = _mapper.Map<User>(entity);
            return updatedUser;
        }

        public async Task<User> GetByIdAsync(Guid? id, CancellationToken cancellationToken = default)
        {
            var entityQuery = _context.Users
                .AsSplitQuery()
                .Include(u => u.Role)
                    .ThenInclude(r => r.Permissions)
                .Include(u => u.BinPhoto)
                    .ThenInclude(bp => bp.BinPhotoBinTypes);
    
            var entity = await entityQuery.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    
            // Логирование для проверки
            Console.WriteLine($"BinPhoto count: {entity?.BinPhoto?.Count ?? 0}");
            if (entity?.BinPhoto != null)
            {
                foreach (var photo in entity.BinPhoto)
                {
                    Console.WriteLine($"Photo ID: {photo.Id}, FileName: {photo.FileName}");
                }
            }

            return _mapper.Map<User>(entity);
        }

        public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .ThenInclude(r => r.Permissions)
                .Include(u => u.BinPhoto)
                .ThenInclude(bp => bp.BinPhotoBinTypes)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            return _mapper.Map<User>(entity);
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

                if (entity is null)
                    throw new KeyNotFoundException();
                
                entity.Firstname = user.Firstname;
                entity.Surname = user.Surname;
                entity.Email = user.Email.Value;
                entity.RoleId = user.RoleId;
                
                entity.AccountEnabled = user.AccountEnabled;
                entity.LockedUntil = user.LockedUntil ??  DateTime.MinValue;
                entity.BlockReason = user.BlockReason;

                //user.Adapt(entity);

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении пользователя {UserId}", user.Id);
                throw;
            }
        }

        public async Task UpdateLastLoggedAtAsync(User user, DateTime date, CancellationToken cancellationToken = default)
        {
            await _context.Users
                .Where(u => u.Id == user.Id)
                .ExecuteUpdateAsync(u => u.SetProperty(
                    p => p.LastLogindAt, date), cancellationToken);
        }

        // public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        // {
        //     var entity = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        //     if (entity != null)
        //     {
        //         _context.Users.Remove(entity);
        //         await _context.SaveChangesAsync(cancellationToken);
        //     }
        
    }
}
