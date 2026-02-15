namespace ZulAi.Domain.Entities;

public class UniverseState
{
    public Guid Id { get; set; }
    public int CurrentTick { get; set; }
    public int GridWidth { get; set; }
    public int GridHeight { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastTickAt { get; set; }

    public ICollection<Atom> Atoms { get; set; } = new List<Atom>();
    public ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();
    public ICollection<GeneratedOutput> Outputs { get; set; } = new List<GeneratedOutput>();
}
