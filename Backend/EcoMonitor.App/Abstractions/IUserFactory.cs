using EcoMonitor.Core.Models.Users;

namespace EcoMonitor.App.Abstractions
{
    public interface IUserFactory
    {
        User Create(string firstname, string surname, string email, string password, string role);
    }
}