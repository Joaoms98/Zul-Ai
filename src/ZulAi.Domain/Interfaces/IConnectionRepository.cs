using ZulAi.Domain.Entities;

namespace ZulAi.Domain.Interfaces;

public interface IConnectionRepository : IRepository<AtomConnection>
{
    Task<IReadOnlyList<AtomConnection>> GetActiveByUniverseAsync(Guid universeId);
    Task<IReadOnlyList<AtomConnection>> GetByAtomAsync(Guid atomId);
}
