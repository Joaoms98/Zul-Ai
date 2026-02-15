namespace ZulAi.Domain.Entities;

public class AtomConnection
{
    public Guid Id { get; set; }
    public Guid SourceAtomId { get; set; }
    public Guid TargetAtomId { get; set; }
    public double Strength { get; set; }
    public int TickFormed { get; set; }
    public bool IsActive { get; set; }

    public Atom SourceAtom { get; set; } = null!;
    public Atom TargetAtom { get; set; } = null!;
}
