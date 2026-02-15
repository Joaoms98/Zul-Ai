using Microsoft.EntityFrameworkCore;
using ZulAi.Domain.Entities;
using ZulAi.Domain.Interfaces;
using ZulAi.Infrastructure.Data;

namespace ZulAi.Infrastructure.Repositories;

public class ConnectionRepository : RepositoryBase<AtomConnection>, IConnectionRepository
{
    public ConnectionRepository(ZulAiDbContext context) : base(context) { }

    public async Task<IReadOnlyList<AtomConnection>> GetActiveByUniverseAsync(Guid universeId)
    {
        return await DbSet
            .Include(c => c.SourceAtom)
            .Include(c => c.TargetAtom)
            .Where(c => c.IsActive && c.SourceAtom.UniverseStateId == universeId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<AtomConnection>> GetByAtomAsync(Guid atomId)
    {
        return await DbSet
            .Where(c => c.IsActive && (c.SourceAtomId == atomId || c.TargetAtomId == atomId))
            .ToListAsync();
    }
}
