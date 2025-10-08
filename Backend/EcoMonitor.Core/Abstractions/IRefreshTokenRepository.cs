using EcoMonitor.Core.Models.Auth;

namespace EcoMonitor.DataAccess.Repositories.Auth;

public interface IRefreshTokenRepository
{
    Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task<RefreshToken> GetByTokenHashAsync(string tokenHash);
    Task RevokeAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RefreshToken>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken =  default);
    Task UpdateAsync(RefreshToken refresherToken, CancellationToken cancellationToken = default);
}