using Microsoft.EntityFrameworkCore;
using ZulAi.Domain.Entities;
using ZulAi.Domain.Interfaces;
using ZulAi.Infrastructure.Data;

namespace ZulAi.Infrastructure.Repositories;

public class AtomRepository : RepositoryBase<Atom>, IAtomRepository
{
    public AtomRepository(ZulAiDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Atom>> GetAliveByUniverseAsync(Guid universeId)
    {
        return await DbSet
            .Where(a => a.UniverseStateId == universeId && a.IsAlive)
            .ToListAsync();
    }

    public async Task<Atom?> GetWithConnectionsAsync(Guid atomId)
    {
        return await DbSet
            .Include(a => a.ConnectionsAsSource.Where(c => c.IsActive))
            .Include(a => a.ConnectionsAsTarget.Where(c => c.IsActive))
            .FirstOrDefaultAsync(a => a.Id == atomId);
    }
}
