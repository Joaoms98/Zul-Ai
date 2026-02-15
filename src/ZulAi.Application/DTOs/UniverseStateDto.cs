namespace ZulAi.Application.DTOs;

public class UniverseStateDto
{
    public Guid Id { get; set; }
    public int CurrentTick { get; set; }
    public int GridWidth { get; set; }
    public int GridHeight { get; set; }
    public bool IsActive { get; set; }
    public int AtomsAlive { get; set; }
    public int TotalConnections { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<AtomDto> Atoms { get; set; } = new();
}
