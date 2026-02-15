using ZulAi.Application.Interfaces;
using ZulAi.Application.Rules;
using ZulAi.Domain.Entities;
using ZulAi.Domain.Enums;

namespace ZulAi.Application.Services;

public class ConnectionRuleEngine : IConnectionRuleEngine
{
    private const double ConnectionThreshold = 0.55;
    private const double BreakDistanceThreshold = 0.1;
    private const double BreakStrengthThreshold = 0.2;

    private readonly IEnumerable<IConnectionRule> _rules;

    public ConnectionRuleEngine(IEnumerable<IConnectionRule> rules)
    {
        _rules = rules;
    }

    public IReadOnlyList<(Atom Source, Atom Target, double Strength)> EvaluateNewConnections(IReadOnlyList<Atom> atoms)
    {
        var results = new List<(Atom, Atom, double)>();
        var alive = atoms.Where(a => a.IsAlive).ToList();

        // Build a set of existing active connections for quick lookup
        var existingPairs = new HashSet<string>();
        foreach (var atom in alive)
        {
            foreach (var conn in atom.ConnectionsAsSource.Where(c => c.IsActive))
                existingPairs.Add(PairKey(conn.SourceAtomId, conn.TargetAtomId));
            foreach (var conn in atom.ConnectionsAsTarget.Where(c => c.IsActive))
                existingPairs.Add(PairKey(conn.SourceAtomId, conn.TargetAtomId));
        }

        for (int i = 0; i < alive.Count; i++)
        {
            for (int j = i + 1; j < alive.Count; j++)
            {
                var key = PairKey(alive[i].Id, alive[j].Id);
                if (existingPairs.Contains(key))
                    continue;

                double totalScore = _rules.Sum(r => r.Evaluate(alive[i], alive[j]) * r.Weight);

                if (totalScore >= ConnectionThreshold)
                    results.Add((alive[i], alive[j], totalScore));
            }
        }

        return results;
    }

    public IReadOnlyList<AtomConnection> EvaluateBreakingConnections(
        IReadOnlyList<AtomConnection> connections, IReadOnlyList<Atom> atoms)
    {
        var atomMap = atoms.ToDictionary(a => a.Id);
        var toBreak = new List<AtomConnection>();

        foreach (var conn in connections.Where(c => c.IsActive))
        {
            if (!atomMap.TryGetValue(conn.SourceAtomId, out var source) ||
                !atomMap.TryGetValue(conn.TargetAtomId, out var target))
            {
                toBreak.Add(conn);
                continue;
            }

            // Break if either atom is dead
            if (!source.IsAlive || !target.IsAlive)
            {
                toBreak.Add(conn);
                continue;
            }

            // Break if atoms drifted too far apart
            var dx = source.PositionX - target.PositionX;
            var dy = source.PositionY - target.PositionY;
            var distance = Math.Sqrt(dx * dx + dy * dy);
            if (distance > 20.0)
            {
                toBreak.Add(conn);
                continue;
            }

            // Void atoms drain connection strength
            if (source.Type == AtomType.Void || target.Type == AtomType.Void)
            {
                conn.Strength -= 0.1;
                if (conn.Strength < BreakStrengthThreshold)
                {
                    toBreak.Add(conn);
                    continue;
                }
            }
        }

        return toBreak;
    }

    private static string PairKey(Guid a, Guid b)
    {
        return a.CompareTo(b) < 0 ? $"{a}:{b}" : $"{b}:{a}";
    }
}
