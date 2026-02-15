using ZulAi.Domain.Enums;

namespace ZulAi.Domain.Entities;

public class Atom
{
    public Guid Id { get; set; }
    public AtomType Type { get; set; }
    public double PositionX { get; set; }
    public double PositionY { get; set; }
    public double Energy { get; set; }
    public int Age { get; set; }
    public bool IsAlive { get; set; }
    public char Symbol { get; set; }
    public Guid UniverseStateId { get; set; }
    public DateTime CreatedAt { get; set; }

    public UniverseState UniverseState { get; set; } = null!;
    public ICollection<AtomConnection> ConnectionsAsSource { get; set; } = new List<AtomConnection>();
    public ICollection<AtomConnection> ConnectionsAsTarget { get; set; } = new List<AtomConnection>();
}
