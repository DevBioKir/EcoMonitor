using EcoMonitor.App.Factory.Users;
using EcoMonitor.Core.Models.Users;
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
            var user = new UserFactory(_passwordHasher);

            var users = new List<User>()
            {
                user.Create(
                    "Ivan",
                    "Ivanov",
                    "ivanov@mail.ry",
                    "23sdqfg5432"),
                user.Create(
                    "Peter",
                    "Petrov",
                    "petrov@mail.ry",
                    "wadsaf341232sad"),
                user.Create(
                    "Peter",
                    "Petrov",
                    "petrov@mail.ru",
                    "wadsaf341232sad")
            };

            var userEntity = _mapper.Map<UserEntity>(users);

            await _context.Users.AddRangeAsync(userEntity);
            await _context.SaveChangesAsync();

            var userRepo = new UserRepository(_context, _mapper);

            var ct = new CancellationToken();
            // Act
            var result = await userRepo.GetAllAsync(ct);

            // Assert
            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            Assert.Contains(result, u => u.Firstname == "Ivanov");
            Assert.Contains(result, u => u.Firstname == "Peter");
            Assert.Contains(result, u => u.Surname == "Ivan");
            Assert.Contains(result, u => u.Surname == "Petrov");
        }
    }
}
