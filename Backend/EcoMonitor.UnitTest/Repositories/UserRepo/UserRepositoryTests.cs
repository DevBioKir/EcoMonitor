using EcoMonitor.App.Factory.Users;
using EcoMonitor.Core.Models.Users;
using EcoMonitor.Core.ValueObjects;
using EcoMonitor.DataAccess.Entities.Users;
using EcoMonitor.DataAccess.Repositories.Users;

namespace EcoMonitor.UnitTest.Repositories.UserRepo
{
    public class UserRepositoryTests : TestBase
    {
        [Fact]
        public async Task GetAllUsers_ReturnsAllUsers()
        {
            // Arrange
            var userFactory = new UserFactory(_passwordHasher);

            var emailIvan = Email.Create("ivanov@mail.ry");

            var emailPeter = Email.Create("petrov@mail.ry");

            var users = new List<User>()
            {
                userFactory.Create(
                    "Ivan",
                    "Ivanov",
                    emailIvan.Value,
                    "23sdqfg5432"),
                userFactory.Create(
                    "Peter",
                    "Petrov",
                    emailPeter.Value,
                    "wadsaf341232sad")
            };

            var roleEntity = new UserRoleEntity
            {
                Id = UserRole.User.Id,
                Name = UserRole.User.Name,
                Description = UserRole.User.Description,
                Permissions = UserRole.User.Permissions
                                .Select(p => new PermissionEntity { Code = p.Code })
                                .ToList()
            };

            await _context.UserRoles.AddAsync(roleEntity);
            await _context.SaveChangesAsync();


            var userEntities = _mapper.Map<List<UserEntity>>(users);
            foreach (var u in userEntities)
            {
                u.RoleId = roleEntity.Id;
                u.Role = roleEntity; // обязательно для Include
            }

            await _context.Users.AddRangeAsync(userEntities);
            var savedCount = await _context.SaveChangesAsync();

            var userRepo = new UserRepository(_context, _mapper);

            var ct = new CancellationToken();
            // Act
            var result = await userRepo.GetAllAsync(ct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, savedCount);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, u => u.Firstname == "Ivan");
            Assert.Contains(result, u => u.Firstname == "Peter");
            Assert.Contains(result, u => u.Surname == "Ivanov");
            Assert.Contains(result, u => u.Surname == "Petrov");
        }

        [Fact]
        public async Task GetUserById_ReturnTheFondUser()
        {
            // Arrange
            var userFactory = new UserFactory(_passwordHasher);

            var emailIvan = Email.Create("ivanov@mail.ry");

            var user = userFactory.Create(
                    "Ivan",
                    "Ivanov",
                    emailIvan.Value,
                    "23sdqfg5432");

            var roleEntity = new UserRoleEntity
            {
                Id = UserRole.User.Id,
                Name = UserRole.User.Name,
                Description = UserRole.User.Description,
                Permissions = UserRole.User.Permissions
                                .Select(p => new PermissionEntity { Code = p.Code })
                                .ToList()
            };

            await _context.UserRoles.AddAsync(roleEntity);
            await _context.SaveChangesAsync();


            var userEntity = _mapper.Map<UserEntity>(user);

            userEntity.RoleId = roleEntity.Id;
            userEntity.Role = roleEntity;

            await _context.Users.AddAsync(userEntity);
            await _context.SaveChangesAsync();

            var userRepo = new UserRepository(_context, _mapper);

            var ct = new CancellationToken();
            // Act
            var result = await userRepo.GetByIdAsync(user.Id, ct);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            Assert.Contains("Ivan", result.Firstname);
            Assert.Contains("Ivanov", result.Surname);
            Assert.Contains("ivanov@mail.ry", result.Email.Value);
            Assert.True(result.CheckPassword("23sdqfg5432", _passwordHasher));
        }
    }
}
