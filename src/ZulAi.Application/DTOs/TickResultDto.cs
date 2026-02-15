namespace ZulAi.Application.DTOs;

public class TickResultDto
{
    public Guid UniverseId { get; set; }
    public int Tick { get; set; }
    public int AtomsAlive { get; set; }
    public int AtomsBorn { get; set; }
    public int AtomsDied { get; set; }
    public int ConnectionsFormed { get; set; }
    public int ConnectionsBroken { get; set; }
    public double TotalEnergy { get; set; }
    public string AsciiArt { get; set; } = string.Empty;
    public List<string> EventLog { get; set; } = new();
}
