using EcoMonitor.App.Abstractions;

namespace EcoMonitor.App.Factory.Users.Resolve;

public class UserRegisterFactoryResolver : IUserRegisterFactoryResolver
{
    private readonly Dictionary<string, IUserRegisterFactory> _factories;

    public UserRegisterFactoryResolver(IEnumerable<IUserRegisterFactory> factories)
    {
        _factories = factories.ToDictionary(f => 
            f.Role.Name, StringComparer.OrdinalIgnoreCase);
    }
    
    public IUserRegisterFactory Resolve(string roleName) =>
    _factories[roleName];
}