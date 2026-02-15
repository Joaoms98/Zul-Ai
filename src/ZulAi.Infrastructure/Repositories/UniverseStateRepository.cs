using Microsoft.EntityFrameworkCore;
using ZulAi.Domain.Entities;
using ZulAi.Domain.Interfaces;
using ZulAi.Infrastructure.Data;

namespace ZulAi.Infrastructure.Repositories;

public class UniverseStateRepository : RepositoryBase<UniverseState>, IUniverseStateRepository
{
    public UniverseStateRepository(ZulAiDbContext context) : base(context) { }

    public async Task<UniverseState?> GetWithAtomsAndConnectionsAsync(Guid id)
    {
        return await DbSet
            .Include(u => u.Atoms.Where(a => a.IsAlive))
                .ThenInclude(a => a.ConnectionsAsSource.Where(c => c.IsActive))
            .Include(u => u.Atoms.Where(a => a.IsAlive))
                .ThenInclude(a => a.ConnectionsAsTarget.Where(c => c.IsActive))
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}
