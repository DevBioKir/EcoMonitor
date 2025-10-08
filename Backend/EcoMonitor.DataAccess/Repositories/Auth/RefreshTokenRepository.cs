using EcoMonitor.Core.Models.Auth;
using EcoMonitor.DataAccess.Entities.Auth;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace EcoMonitor.DataAccess.Repositories.Auth;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly EcoMonitorDbContext _context;
    private readonly IMapper _mapper;

    public RefreshTokenRepository(
        EcoMonitorDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        var tokenEntity = _mapper.Map<RefreshTokenEntity>(refreshToken);
        
        _context.RefreshTokens.Add(tokenEntity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<RefreshToken> GetByTokenHashAsync(string tokenHash)
    {
        var tokenEntity = await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.TokenHash == tokenHash
                                      && !r.Revoked && r.ExpireAt > DateTime.UtcNow);
        
        if (tokenEntity == null) 
            return null;
        
        return _mapper.Map<RefreshToken>(tokenEntity);
    }

    public async Task RevokeAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        refreshToken.Revoke();
        
        var tokenEntity = _mapper.Map<RefreshTokenEntity>(refreshToken);
        
        _context.RefreshTokens.Update(tokenEntity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RefreshToken>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken =  default)
    {
        var tokenEntity =  await _context.RefreshTokens
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .ToListAsync(cancellationToken);
        
        return _mapper.Map<List<RefreshToken>>(tokenEntity);
    }

    public async Task UpdateAsync(RefreshToken refresherToken, CancellationToken cancellationToken = default)
    {
        var tokenEntity = _mapper.Map<RefreshTokenEntity>(refresherToken);
        _context.RefreshTokens.Update(tokenEntity);
        
        await _context.SaveChangesAsync(cancellationToken);
    }
}