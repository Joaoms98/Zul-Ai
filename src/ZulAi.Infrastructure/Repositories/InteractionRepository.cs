using Microsoft.EntityFrameworkCore;
using ZulAi.Domain.Entities;
using ZulAi.Domain.Enums;
using ZulAi.Domain.Interfaces;
using ZulAi.Infrastructure.Data;

namespace ZulAi.Infrastructure.Repositories;

public class InteractionRepository : RepositoryBase<Interaction>, IInteractionRepository
{
    public InteractionRepository(ZulAiDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Interaction>> GetByUniverseAsync(Guid universeId, InteractionType? type, int skip, int take)
    {
        var query = DbSet.Where(i => i.UniverseStateId == universeId);
        if (type.HasValue)
            query = query.Where(i => i.Type == type.Value);

        return await query
            .OrderByDescending(i => i.Tick)
            .ThenByDescending(i => i.OccurredAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> CountByUniverseAsync(Guid universeId, InteractionType? type)
    {
        var query = DbSet.Where(i => i.UniverseStateId == universeId);
        if (type.HasValue)
            query = query.Where(i => i.Type == type.Value);

        return await query.CountAsync();
    }
}
