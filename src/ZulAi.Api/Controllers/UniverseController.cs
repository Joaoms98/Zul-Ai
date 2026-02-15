using Microsoft.AspNetCore.Mvc;
using ZulAi.Application.Interfaces;

namespace ZulAi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UniverseController : ControllerBase
{
    private readonly IUniverseService _universeService;

    public UniverseController(IUniverseService universeService)
    {
        _universeService = universeService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUniverseRequest? request)
    {
        var result = await _universeService.CreateUniverseAsync(
            request?.Width ?? 80,
            request?.Height ?? 40,
            request?.InitialAtoms ?? 20);
        return CreatedAtAction(nameof(GetState), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetState(Guid id)
    {
        try
        {
            var result = await _universeService.GetStateAsync(id);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id:guid}/tick")]
    public async Task<IActionResult> Tick(Guid id)
    {
        try
        {
            var result = await _universeService.TickAsync(id);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/tick/{count:int}")]
    public async Task<IActionResult> TickMultiple(Guid id, int count)
    {
        if (count < 1 || count > 100)
            return BadRequest(new { error = "Count must be between 1 and 100" });

        try
        {
            var result = await _universeService.TickMultipleAsync(id, count);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id:guid}/analytics")]
    public async Task<IActionResult> GetAnalytics(Guid id)
    {
        try
        {
            var result = await _universeService.GetAnalyticsAsync(id);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

public class CreateUniverseRequest
{
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? InitialAtoms { get; set; }
}
