using ZulAi.Domain.Entities;
using ZulAi.Domain.Enums;

namespace ZulAi.Application.Rules;

public class TypeAffinityRule : IConnectionRule
{
    public double Weight => 0.3;

    // Affinity matrix: [source, target]
    // Luminar=0, Umbral=1, Nexus=2, Pulsar=3, Void=4
    private static readonly double[,] AffinityMatrix = new double[,]
    {
        //           Luminar  Umbral  Nexus  Pulsar  Void
        /* Luminar */ { 0.2,   0.8,   0.6,   0.5,   0.1 },
        /* Umbral  */ { 0.8,   0.1,   0.5,   0.3,   0.7 },
        /* Nexus   */ { 0.6,   0.5,   0.4,   0.7,   0.3 },
        /* Pulsar  */ { 0.5,   0.3,   0.7,   0.2,   0.4 },
        /* Void    */ { 0.1,   0.7,   0.3,   0.4,   0.0 },
    };

    public double Evaluate(Atom a, Atom b)
    {
        return AffinityMatrix[(int)a.Type, (int)b.Type];
    }
}
