using ZulAi.Domain.Entities;

namespace ZulAi.Application.Interfaces;

public interface IConnectionRuleEngine
{
    IReadOnlyList<(Atom Source, Atom Target, double Strength)> EvaluateNewConnections(IReadOnlyList<Atom> atoms);
    IReadOnlyList<AtomConnection> EvaluateBreakingConnections(IReadOnlyList<AtomConnection> connections, IReadOnlyList<Atom> atoms);
}
