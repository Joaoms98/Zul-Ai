using Microsoft.AspNetCore.Mvc;
using ZulAi.Domain.Enums;
using ZulAi.Domain.Interfaces;

namespace ZulAi.Api.Controllers;

[ApiController]
[Route("api/universe/{universeId:guid}/history")]
public class HistoryController : ControllerBase
{
    private readonly IInteractionRepository _interactionRepo;

    public HistoryController(IInteractionRepository interactionRepo)
    {
        _interactionRepo = interactionRepo;
    }

    [HttpGet]
    public async Task<IActionResult> GetHistory(
        Guid universeId,
        [FromQuery] InteractionType? type,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        var interactions = await _interactionRepo.GetByUniverseAsync(universeId, type, skip, take);
        var total = await _interactionRepo.CountByUniverseAsync(universeId, type);

        return Ok(new
        {
            Total = total,
            Skip = skip,
            Take = take,
            Items = interactions.Select(i => new
            {
                i.Id,
                Type = i.Type.ToString(),
                i.AtomId,
                i.ConnectionId,
                i.Tick,
                i.Description,
                i.OccurredAt
            })
        });
    }
}
