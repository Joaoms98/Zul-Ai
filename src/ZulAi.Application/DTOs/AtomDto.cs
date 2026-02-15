using ZulAi.Domain.Enums;

namespace ZulAi.Application.DTOs;

public class AtomDto
{
    public Guid Id { get; set; }
    public AtomType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public double PositionX { get; set; }
    public double PositionY { get; set; }
    public double Energy { get; set; }
    public int Age { get; set; }
    public bool IsAlive { get; set; }
    public char Symbol { get; set; }
    public int ConnectionCount { get; set; }
}
