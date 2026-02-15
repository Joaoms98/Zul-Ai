using Microsoft.AspNetCore.Mvc;
using ZulAi.Application.Interfaces;

namespace ZulAi.Api.Controllers;

[ApiController]
[Route("api/universe/{universeId:guid}/ascii")]
public class AsciiArtController : ControllerBase
{
    private readonly IUniverseService _universeService;

    public AsciiArtController(IUniverseService universeService)
    {
        _universeService = universeService;
    }

    [HttpGet]
    [Produces("text/plain")]
    public async Task<IActionResult> GetCurrent(Guid universeId)
    {
        try
        {
            var result = await _universeService.GetCurrentAsciiAsync(universeId);
            return Content(result.AsciiArt, "text/plain");
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("json")]
    public async Task<IActionResult> GetCurrentJson(Guid universeId)
    {
        try
        {
            var result = await _universeService.GetCurrentAsciiAsync(universeId);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory(Guid universeId, [FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        var result = await _universeService.GetHistoryAsync(universeId, skip, take);
        return Ok(result);
    }
}
