using Microsoft.EntityFrameworkCore;
using ZulAi.Domain.Entities;
using ZulAi.Domain.Interfaces;
using ZulAi.Infrastructure.Data;

namespace ZulAi.Infrastructure.Repositories;

public class GeneratedOutputRepository : RepositoryBase<GeneratedOutput>, IGeneratedOutputRepository
{
    public GeneratedOutputRepository(ZulAiDbContext context) : base(context) { }

    public async Task<GeneratedOutput?> GetByTickAsync(Guid universeId, int tick)
    {
        return await DbSet
            .FirstOrDefaultAsync(o => o.UniverseStateId == universeId && o.Tick == tick);
    }

    public async Task<GeneratedOutput?> GetLatestAsync(Guid universeId)
    {
        return await DbSet
            .Where(o => o.UniverseStateId == universeId)
            .OrderByDescending(o => o.Tick)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<GeneratedOutput>> GetHistoryAsync(Guid universeId, int skip, int take)
    {
        return await DbSet
            .Where(o => o.UniverseStateId == universeId)
            .OrderByDescending(o => o.Tick)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }
}
