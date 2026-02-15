using ZulAi.Application.DTOs;

namespace ZulAi.Application.Interfaces;

public interface IUniverseService
{
    Task<UniverseStateDto> CreateUniverseAsync(int width = 80, int height = 40, int initialAtoms = 20);
    Task<TickResultDto> TickAsync(Guid universeId);
    Task<TickResultDto> TickMultipleAsync(Guid universeId, int count);
    Task<UniverseStateDto> GetStateAsync(Guid universeId);
    Task<AsciiOutputDto> GetCurrentAsciiAsync(Guid universeId);
    Task<IReadOnlyList<AsciiOutputDto>> GetHistoryAsync(Guid universeId, int skip = 0, int take = 50);
    Task<AnalyticsDto> GetAnalyticsAsync(Guid universeId);
}
