using ZulAi.Domain.Entities;

namespace ZulAi.Domain.Interfaces;

public interface IGeneratedOutputRepository : IRepository<GeneratedOutput>
{
    Task<GeneratedOutput?> GetByTickAsync(Guid universeId, int tick);
    Task<GeneratedOutput?> GetLatestAsync(Guid universeId);
    Task<IReadOnlyList<GeneratedOutput>> GetHistoryAsync(Guid universeId, int skip, int take);
}
