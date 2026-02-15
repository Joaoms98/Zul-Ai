using ZulAi.Domain.Enums;

namespace ZulAi.Domain.Entities;

public class Interaction
{
    public Guid Id { get; set; }
    public InteractionType Type { get; set; }
    public Guid? AtomId { get; set; }
    public Guid? ConnectionId { get; set; }
    public int Tick { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public Guid UniverseStateId { get; set; }

    public UniverseState UniverseState { get; set; } = null!;
}
