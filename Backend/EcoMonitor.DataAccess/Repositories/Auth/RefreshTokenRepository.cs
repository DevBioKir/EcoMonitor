using EcoMonitor.Core.Models.Auth;
using EcoMonitor.DataAccess.Entities.Auth;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.IO;

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
        var tokenEntity = await _context.RefreshTokens.FindAsync(refreshToken.Id);
        if (tokenEntity == null)
            throw new Exception("RefreshToken not found");

        tokenEntity.Revoked = true;
    
        await _context.SaveChangesAsync(cancellationToken);
        
        // refreshToken.Revoke();
        //
        // var tokenEntity = _mapper.Map<RefreshTokenEntity>(refreshToken);
        //
        // _context.RefreshTokens.Update(tokenEntity);
        // await _context.SaveChangesAsync(cancellationToken);
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
        var tokenEntity = await _context.RefreshTokens.FindAsync(refresherToken.Id);
        if (tokenEntity == null)
            throw new Exception("RefreshToken not found");

        // Обновляем только нужные поля
        tokenEntity.Revoked = refresherToken.Revoked;
        tokenEntity.ExpireAt = refresherToken.ExpireAt;
        // Обнови другие необходимые поля

        await _context.SaveChangesAsync(cancellationToken);
        
        // var tokenEntity = _mapper.Map<RefreshTokenEntity>(refresherToken);
        // _context.RefreshTokens.Update(tokenEntity);
        //
        // await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<RefreshToken?> GetValidRefreshTokenByUserIdAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        var now =  DateTime.UtcNow;

        var token = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.ExpireAt > now && !rt.Revoked)
            .OrderByDescending(rt => rt.IssuedAt)
            .FirstOrDefaultAsync(cancellationToken);
        
        return _mapper.Map<RefreshToken>(token);
    }
}