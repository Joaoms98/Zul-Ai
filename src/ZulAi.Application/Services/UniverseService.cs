using ZulAi.Application.DTOs;
using ZulAi.Application.Interfaces;
using ZulAi.Domain.Entities;
using ZulAi.Domain.Enums;
using ZulAi.Domain.Interfaces;

namespace ZulAi.Application.Services;

public class UniverseService : IUniverseService
{
    private static readonly ThreadLocal<Random> Rng = new(() => new Random());

    private readonly IUniverseStateRepository _universeRepo;
    private readonly IAtomRepository _atomRepo;
    private readonly IConnectionRepository _connectionRepo;
    private readonly IInteractionRepository _interactionRepo;
    private readonly IGeneratedOutputRepository _outputRepo;
    private readonly IAtomFactory _atomFactory;
    private readonly IConnectionRuleEngine _ruleEngine;
    private readonly IAsciiRenderer _renderer;

    // Type-dependent movement speeds
    private static readonly Dictionary<AtomType, double> MovementSpeed = new()
    {
        { AtomType.Luminar, 1.5 },
        { AtomType.Umbral, 0.8 },
        { AtomType.Nexus, 0.5 },
        { AtomType.Pulsar, 2.0 },
        { AtomType.Void, 0.3 },
    };

    // Type-dependent max lifespan (in ticks)
    private static readonly Dictionary<AtomType, int> MaxLifespan = new()
    {
        { AtomType.Luminar, 150 },
        { AtomType.Umbral, 200 },
        { AtomType.Nexus, 250 },
        { AtomType.Pulsar, 80 },
        { AtomType.Void, 300 },
    };

    public UniverseService(
        IUniverseStateRepository universeRepo,
        IAtomRepository atomRepo,
        IConnectionRepository connectionRepo,
        IInteractionRepository interactionRepo,
        IGeneratedOutputRepository outputRepo,
        IAtomFactory atomFactory,
        IConnectionRuleEngine ruleEngine,
        IAsciiRenderer renderer)
    {
        _universeRepo = universeRepo;
        _atomRepo = atomRepo;
        _connectionRepo = connectionRepo;
        _interactionRepo = interactionRepo;
        _outputRepo = outputRepo;
        _atomFactory = atomFactory;
        _ruleEngine = ruleEngine;
        _renderer = renderer;
    }

    public async Task<UniverseStateDto> CreateUniverseAsync(int width = 80, int height = 40, int initialAtoms = 20)
    {
        var universe = new UniverseState
        {
            Id = Guid.NewGuid(),
            CurrentTick = 0,
            GridWidth = width,
            GridHeight = height,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            LastTickAt = DateTime.UtcNow
        };

        await _universeRepo.AddAsync(universe);

        var atoms = new List<Atom>();
        var interactions = new List<Interaction>();
        for (int i = 0; i < initialAtoms; i++)
        {
            var atom = _atomFactory.CreateRandom(width, height, universe.Id);
            atoms.Add(atom);
            interactions.Add(new Interaction
            {
                Id = Guid.NewGuid(),
                Type = InteractionType.Born,
                AtomId = atom.Id,
                Tick = 0,
                Description = $"{atom.Type} nasceu em ({atom.PositionX:F1}, {atom.PositionY:F1})",
                OccurredAt = DateTime.UtcNow,
                UniverseStateId = universe.Id
            });
        }

        await _atomRepo.AddRangeAsync(atoms);
        await _interactionRepo.AddRangeAsync(interactions);

        // Render initial state
        var ascii = _renderer.Render(width, height, atoms, new List<AtomConnection>());
        var output = new GeneratedOutput
        {
            Id = Guid.NewGuid(),
            Tick = 0,
            AsciiArt = ascii,
            GridWidth = width,
            GridHeight = height,
            AtomCount = atoms.Count,
            ConnectionCount = 0,
            GeneratedAt = DateTime.UtcNow,
            UniverseStateId = universe.Id
        };
        await _outputRepo.AddAsync(output);

        await _universeRepo.SaveChangesAsync();

        return MapToDto(universe, atoms);
    }

    public async Task<TickResultDto> TickAsync(Guid universeId)
    {
        var universe = await _universeRepo.GetWithAtomsAndConnectionsAsync(universeId)
            ?? throw new KeyNotFoundException($"Universe {universeId} not found");

        if (!universe.IsActive)
            throw new InvalidOperationException("Universe is no longer active");

        var rng = Rng.Value!;
        var tick = universe.CurrentTick + 1;
        var allAtoms = universe.Atoms.ToList();
        var aliveAtoms = allAtoms.Where(a => a.IsAlive).ToList();
        var activeConnections = await _connectionRepo.GetActiveByUniverseAsync(universeId);
        var mutableConnections = activeConnections.ToList();

        var eventLog = new List<string>();
        var newInteractions = new List<Interaction>();
        int born = 0, died = 0, connected = 0, broken = 0;

        // 1. SPAWN — fewer atoms = higher spawn chance
        var spawnChance = Math.Max(0.1, 1.0 - (aliveAtoms.Count / 50.0));
        if (rng.NextDouble() < spawnChance)
        {
            int spawnCount = rng.Next(1, 4);
            for (int i = 0; i < spawnCount; i++)
            {
                var newAtom = _atomFactory.CreateRandom(universe.GridWidth, universe.GridHeight, universeId);
                await _atomRepo.AddAsync(newAtom);
                aliveAtoms.Add(newAtom);
                allAtoms.Add(newAtom);
                born++;

                var msg = $"{newAtom.Type} nasceu em ({newAtom.PositionX:F1}, {newAtom.PositionY:F1})";
                eventLog.Add(msg);
                newInteractions.Add(CreateInteraction(InteractionType.Born, newAtom.Id, null, tick, msg, universeId));
            }
        }

        // 2. MOVE — each atom drifts randomly based on type
        foreach (var atom in aliveAtoms)
        {
            var speed = MovementSpeed[atom.Type];
            atom.PositionX += (rng.NextDouble() * 2.0 - 1.0) * speed;
            atom.PositionY += (rng.NextDouble() * 2.0 - 1.0) * speed;

            // Wrap within grid bounds (inside border)
            atom.PositionX = Math.Clamp(atom.PositionX, 1.0, universe.GridWidth - 2.0);
            atom.PositionY = Math.Clamp(atom.PositionY, 1.0, universe.GridHeight - 2.0);
        }

        // 3. AGE — increment age, kill old atoms
        foreach (var atom in aliveAtoms.ToList())
        {
            atom.Age++;

            if (atom.Age >= MaxLifespan[atom.Type] || atom.Energy <= 0)
            {
                atom.IsAlive = false;
                died++;

                var reason = atom.Energy <= 0 ? "sem energia" : "velho demais";
                var msg = $"{atom.Type} morreu ({reason}) com idade {atom.Age}";
                eventLog.Add(msg);
                newInteractions.Add(CreateInteraction(InteractionType.Died, atom.Id, null, tick, msg, universeId));
                aliveAtoms.Remove(atom);
            }
        }

        // 4. ENERGY — Luminar gives, Void drains, all lose a bit
        foreach (var atom in aliveAtoms)
        {
            // Base energy drain
            atom.Energy -= 0.5;

            // Luminar radiates energy to nearby atoms
            if (atom.Type == AtomType.Luminar)
            {
                foreach (var other in aliveAtoms.Where(o => o.Id != atom.Id))
                {
                    var dist = Distance(atom, other);
                    if (dist < 10.0)
                    {
                        var transfer = 0.3 * (1.0 - dist / 10.0);
                        other.Energy = Math.Min(100.0, other.Energy + transfer);
                    }
                }
            }

            // Void drains nearby atoms
            if (atom.Type == AtomType.Void)
            {
                foreach (var other in aliveAtoms.Where(o => o.Id != atom.Id))
                {
                    var dist = Distance(atom, other);
                    if (dist < 8.0)
                    {
                        var drain = 0.5 * (1.0 - dist / 8.0);
                        other.Energy -= drain;
                        atom.Energy = Math.Min(100.0, atom.Energy + drain * 0.5);
                    }
                }
            }

            // Pulsar energy oscillation
            if (atom.Type == AtomType.Pulsar)
            {
                atom.Energy += Math.Sin(atom.Age * 0.3) * 2.0;
            }

            atom.Energy = Math.Clamp(atom.Energy, 0.0, 100.0);
        }

        // 5. CONNECT — evaluate new connections
        var newConnections = _ruleEngine.EvaluateNewConnections(aliveAtoms);
        foreach (var (source, target, strength) in newConnections)
        {
            var conn = new AtomConnection
            {
                Id = Guid.NewGuid(),
                SourceAtomId = source.Id,
                TargetAtomId = target.Id,
                Strength = strength,
                TickFormed = tick,
                IsActive = true
            };
            await _connectionRepo.AddAsync(conn);
            mutableConnections.Add(conn);
            connected++;

            var msg = $"{source.Type}↔{target.Type} conectados (força: {strength:F2})";
            eventLog.Add(msg);
            newInteractions.Add(CreateInteraction(InteractionType.Connected, source.Id, conn.Id, tick, msg, universeId));
        }

        // 6. DISCONNECT — break weak/dead connections
        var toBreak = _ruleEngine.EvaluateBreakingConnections(mutableConnections, aliveAtoms);
        foreach (var conn in toBreak)
        {
            conn.IsActive = false;
            broken++;

            var msg = $"Conexão quebrada (força: {conn.Strength:F2})";
            eventLog.Add(msg);
            newInteractions.Add(CreateInteraction(InteractionType.Disconnected, null, conn.Id, tick, msg, universeId));
        }

        // 7. LOG — save interactions
        if (newInteractions.Count > 0)
            await _interactionRepo.AddRangeAsync(newInteractions);

        // 8. RENDER — generate ASCII art
        var activeConns = mutableConnections.Where(c => c.IsActive).ToList();
        var asciiArt = _renderer.Render(universe.GridWidth, universe.GridHeight, aliveAtoms, activeConns);

        var genOutput = new GeneratedOutput
        {
            Id = Guid.NewGuid(),
            Tick = tick,
            AsciiArt = asciiArt,
            GridWidth = universe.GridWidth,
            GridHeight = universe.GridHeight,
            AtomCount = aliveAtoms.Count,
            ConnectionCount = activeConns.Count,
            GeneratedAt = DateTime.UtcNow,
            UniverseStateId = universeId
        };
        await _outputRepo.AddAsync(genOutput);

        // 9. PERSIST
        universe.CurrentTick = tick;
        universe.LastTickAt = DateTime.UtcNow;
        _universeRepo.Update(universe);
        await _universeRepo.SaveChangesAsync();

        return new TickResultDto
        {
            UniverseId = universeId,
            Tick = tick,
            AtomsAlive = aliveAtoms.Count,
            AtomsBorn = born,
            AtomsDied = died,
            ConnectionsFormed = connected,
            ConnectionsBroken = broken,
            TotalEnergy = aliveAtoms.Sum(a => a.Energy),
            AsciiArt = asciiArt,
            EventLog = eventLog
        };
    }

    public async Task<TickResultDto> TickMultipleAsync(Guid universeId, int count)
    {
        TickResultDto? result = null;
        for (int i = 0; i < count; i++)
            result = await TickAsync(universeId);

        return result ?? throw new InvalidOperationException("Count must be > 0");
    }

    public async Task<UniverseStateDto> GetStateAsync(Guid universeId)
    {
        var universe = await _universeRepo.GetWithAtomsAndConnectionsAsync(universeId)
            ?? throw new KeyNotFoundException($"Universe {universeId} not found");

        return MapToDto(universe, universe.Atoms.ToList());
    }

    public async Task<AsciiOutputDto> GetCurrentAsciiAsync(Guid universeId)
    {
        var output = await _outputRepo.GetLatestAsync(universeId)
            ?? throw new KeyNotFoundException($"No output found for universe {universeId}");

        return new AsciiOutputDto
        {
            Tick = output.Tick,
            AsciiArt = output.AsciiArt,
            AtomCount = output.AtomCount,
            ConnectionCount = output.ConnectionCount,
            GeneratedAt = output.GeneratedAt
        };
    }

    public async Task<IReadOnlyList<AsciiOutputDto>> GetHistoryAsync(Guid universeId, int skip = 0, int take = 50)
    {
        var outputs = await _outputRepo.GetHistoryAsync(universeId, skip, take);
        return outputs.Select(o => new AsciiOutputDto
        {
            Tick = o.Tick,
            AsciiArt = o.AsciiArt,
            AtomCount = o.AtomCount,
            ConnectionCount = o.ConnectionCount,
            GeneratedAt = o.GeneratedAt
        }).ToList();
    }

    public async Task<AnalyticsDto> GetAnalyticsAsync(Guid universeId)
    {
        var universe = await _universeRepo.GetByIdAsync(universeId)
            ?? throw new KeyNotFoundException($"Universe {universeId} not found");

        var totalBorn = await _interactionRepo.CountByUniverseAsync(universeId, InteractionType.Born);
        var totalDied = await _interactionRepo.CountByUniverseAsync(universeId, InteractionType.Died);
        var totalConnected = await _interactionRepo.CountByUniverseAsync(universeId, InteractionType.Connected);
        var totalDisconnected = await _interactionRepo.CountByUniverseAsync(universeId, InteractionType.Disconnected);

        var aliveAtoms = await _atomRepo.GetAliveByUniverseAsync(universeId);
        var activeConns = await _connectionRepo.GetActiveByUniverseAsync(universeId);

        var byType = aliveAtoms
            .GroupBy(a => a.Type)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());

        return new AnalyticsDto
        {
            UniverseId = universeId,
            CurrentTick = universe.CurrentTick,
            TotalAtomsBorn = totalBorn,
            TotalAtomsDied = totalDied,
            AtomsAlive = aliveAtoms.Count,
            TotalConnectionsFormed = totalConnected,
            TotalConnectionsBroken = totalDisconnected,
            ActiveConnections = activeConns.Count,
            AverageEnergy = aliveAtoms.Count > 0 ? aliveAtoms.Average(a => a.Energy) : 0,
            AtomsByType = byType
        };
    }

    private static Interaction CreateInteraction(InteractionType type, Guid? atomId, Guid? connId, int tick, string desc, Guid universeId)
    {
        return new Interaction
        {
            Id = Guid.NewGuid(),
            Type = type,
            AtomId = atomId,
            ConnectionId = connId,
            Tick = tick,
            Description = desc,
            OccurredAt = DateTime.UtcNow,
            UniverseStateId = universeId
        };
    }

    private static UniverseStateDto MapToDto(UniverseState universe, IList<Atom> atoms)
    {
        var alive = atoms.Where(a => a.IsAlive).ToList();
        return new UniverseStateDto
        {
            Id = universe.Id,
            CurrentTick = universe.CurrentTick,
            GridWidth = universe.GridWidth,
            GridHeight = universe.GridHeight,
            IsActive = universe.IsActive,
            AtomsAlive = alive.Count,
            TotalConnections = alive.SelectMany(a => a.ConnectionsAsSource.Where(c => c.IsActive)).Count(),
            CreatedAt = universe.CreatedAt,
            Atoms = alive.Select(a => new AtomDto
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
                ConnectionCount = a.ConnectionsAsSource.Count(c => c.IsActive) + a.ConnectionsAsTarget.Count(c => c.IsActive)
            }).ToList()
        };
    }

    private static double Distance(Atom a, Atom b)
    {
        var dx = a.PositionX - b.PositionX;
        var dy = a.PositionY - b.PositionY;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}
