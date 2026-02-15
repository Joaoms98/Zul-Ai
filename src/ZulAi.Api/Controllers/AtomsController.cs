using Microsoft.AspNetCore.Mvc;
using ZulAi.Application.DTOs;
using ZulAi.Domain.Enums;
using ZulAi.Domain.Interfaces;

namespace ZulAi.Api.Controllers;

[ApiController]
[Route("api/universe/{universeId:guid}/atoms")]
public class AtomsController : ControllerBase
{
    private readonly IAtomRepository _atomRepo;
    private readonly IConnectionRepository _connectionRepo;

    public AtomsController(IAtomRepository atomRepo, IConnectionRepository connectionRepo)
    {
        _atomRepo = atomRepo;
        _connectionRepo = connectionRepo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAtoms(Guid universeId, [FromQuery] AtomType? type, [FromQuery] bool? alive)
    {
        var atoms = await _atomRepo.GetAliveByUniverseAsync(universeId);
        var filtered = atoms.AsEnumerable();

        if (type.HasValue)
            filtered = filtered.Where(a => a.Type == type.Value);
        if (alive.HasValue)
            filtered = filtered.Where(a => a.IsAlive == alive.Value);

        var result = filtered.Select(a => new AtomDto
        {
            Id = a.Id,
            Type = a.Type,
            TypeName = a.Type.ToString(),
            PositionX = a.PositionX,
            PositionY = a.PositionY,
            Energy = a.Energy,
            Age = a.Age,
            IsAlive = a.IsAlive,
            Symbol = a.Symbol,
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{atomId:guid}")]
    public async Task<IActionResult> GetAtom(Guid universeId, Guid atomId)
    {
        var atom = await _atomRepo.GetWithConnectionsAsync(atomId);
        if (atom == null || atom.UniverseStateId != universeId)
            return NotFound();

        return Ok(new AtomDto
        {
            Id = atom.Id,
            Type = atom.Type,
            TypeName = atom.Type.ToString(),
            PositionX = atom.PositionX,
            PositionY = atom.PositionY,
            Energy = atom.Energy,
            Age = atom.Age,
            IsAlive = atom.IsAlive,
            Symbol = atom.Symbol,
            ConnectionCount = atom.ConnectionsAsSource.Count + atom.ConnectionsAsTarget.Count
        });
    }

    [HttpGet("{atomId:guid}/connections")]
    public async Task<IActionResult> GetConnections(Guid universeId, Guid atomId)
    {
        var atom = await _atomRepo.GetByIdAsync(atomId);
        if (atom == null || atom.UniverseStateId != universeId)
            return NotFound();

        var connections = await _connectionRepo.GetByAtomAsync(atomId);
        return Ok(connections.Select(c => new
        {
            c.Id,
            c.SourceAtomId,
            c.TargetAtomId,
            c.Strength,
            c.TickFormed,
            c.IsActive
        }));
    }
}
