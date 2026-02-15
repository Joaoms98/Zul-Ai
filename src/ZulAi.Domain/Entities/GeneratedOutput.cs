namespace ZulAi.Domain.Entities;

public class GeneratedOutput
{
    public Guid Id { get; set; }
    public int Tick { get; set; }
    public string AsciiArt { get; set; } = string.Empty;
    public int GridWidth { get; set; }
    public int GridHeight { get; set; }
    public int AtomCount { get; set; }
    public int ConnectionCount { get; set; }
    public DateTime GeneratedAt { get; set; }
    public Guid UniverseStateId { get; set; }

    public UniverseState UniverseState { get; set; } = null!;
}
