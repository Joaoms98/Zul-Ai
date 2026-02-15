namespace ZulAi.Application.DTOs;

public class AnalyticsDto
{
    public Guid UniverseId { get; set; }
    public int CurrentTick { get; set; }
    public int TotalAtomsBorn { get; set; }
    public int TotalAtomsDied { get; set; }
    public int AtomsAlive { get; set; }
    public int TotalConnectionsFormed { get; set; }
    public int TotalConnectionsBroken { get; set; }
    public int ActiveConnections { get; set; }
    public double AverageEnergy { get; set; }
    public Dictionary<string, int> AtomsByType { get; set; } = new();
}
