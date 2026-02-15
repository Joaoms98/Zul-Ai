using ZulAi.Domain.Entities;
using ZulAi.Domain.Enums;

namespace ZulAi.Domain.Interfaces;

public interface IInteractionRepository : IRepository<Interaction>
{
    Task<IReadOnlyList<Interaction>> GetByUniverseAsync(Guid universeId, InteractionType? type, int skip, int take);
    Task<int> CountByUniverseAsync(Guid universeId, InteractionType? type);
}
