using ZulAi.Application.Interfaces;
using ZulAi.Domain.Entities;
using ZulAi.Domain.Enums;

namespace ZulAi.Application.Services;

public class AtomFactory : IAtomFactory
{
    private static readonly ThreadLocal<Random> Rng = new(() => new Random());

    private static readonly Dictionary<AtomType, char> Symbols = new()
    {
        { AtomType.Luminar, '*' },
        { AtomType.Umbral, '#' },
        { AtomType.Nexus, '@' },
        { AtomType.Pulsar, '~' },
        { AtomType.Void, '.' },
    };

    private static readonly Dictionary<AtomType, (double MinEnergy, double MaxEnergy)> EnergyRanges = new()
    {
        { AtomType.Luminar, (60.0, 100.0) },
        { AtomType.Umbral, (30.0, 70.0) },
        { AtomType.Nexus, (40.0, 80.0) },
        { AtomType.Pulsar, (50.0, 95.0) },
        { AtomType.Void, (10.0, 40.0) },
    };

    public Atom CreateRandom(int gridWidth, int gridHeight, Guid universeId)
    {
        var rng = Rng.Value!;
        var types = Enum.GetValues<AtomType>();
        var type = types[rng.Next(types.Length)];
        var (minE, maxE) = EnergyRanges[type];

        return new Atom
        {
            Id = Guid.NewGuid(),
            Type = type,
            PositionX = rng.NextDouble() * (gridWidth - 2) + 1,
            PositionY = rng.NextDouble() * (gridHeight - 2) + 1,
            Energy = minE + rng.NextDouble() * (maxE - minE),
            Age = 0,
            IsAlive = true,
            Symbol = Symbols[type],
            UniverseStateId = universeId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public char GetSymbol(AtomType type) => Symbols.GetValueOrDefault(type, '?');
}
