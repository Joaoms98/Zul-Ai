using ZulAi.Domain.Entities;

namespace ZulAi.Domain.Interfaces;

public interface IUniverseStateRepository : IRepository<UniverseState>
{
    Task<UniverseState?> GetWithAtomsAndConnectionsAsync(Guid id);
}
