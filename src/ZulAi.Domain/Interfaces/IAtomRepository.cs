using ZulAi.Domain.Entities;

namespace ZulAi.Domain.Interfaces;

public interface IAtomRepository : IRepository<Atom>
{
    Task<IReadOnlyList<Atom>> GetAliveByUniverseAsync(Guid universeId);
    Task<Atom?> GetWithConnectionsAsync(Guid atomId);
}
