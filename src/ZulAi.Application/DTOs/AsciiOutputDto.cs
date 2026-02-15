namespace ZulAi.Application.DTOs;

public class AsciiOutputDto
{
    public int Tick { get; set; }
    public string AsciiArt { get; set; } = string.Empty;
    public int AtomCount { get; set; }
    public int ConnectionCount { get; set; }
    public DateTime GeneratedAt { get; set; }
}
