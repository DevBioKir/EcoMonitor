namespace EcoMonitor.AdminPanel.Infrastucture.TokenPersistence;

public interface ITokenPersistenceService
{
    Task<(string access, string refresh)> LoadTokensAsync();
    Task SaveAsync();
    Task ClearAsync();
}