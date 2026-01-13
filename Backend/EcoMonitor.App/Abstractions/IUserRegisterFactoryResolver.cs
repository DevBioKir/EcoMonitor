namespace EcoMonitor.App.Abstractions;

public interface IUserRegisterFactoryResolver
{
    IUserRegisterFactory Resolve(string roleName);
}