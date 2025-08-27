using EcoMonitor.App.Abstractions;
using EcoMonitor.Core.Models.Users;
using EcoMonitor.Infrastracture.Abstractions;

namespace EcoMonitor.App.Factory.Users
{
    public class UserFactory : IUserFactory
    {
        private readonly IPasswordHasher _passwordHasher;

        public UserFactory(IPasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public User Create(string firstname, string surname, string email, string password, string role)
        {
            return User.Create(firstname, surname, email, password, _passwordHasher);
        }
    }
}
