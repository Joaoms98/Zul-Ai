using ZulAi.Domain.Entities;

namespace ZulAi.Application.Rules;

public class ProximityRule : IConnectionRule
{
    private const double MaxDistance = 15.0;

    public double Weight => 0.4;

    public double Evaluate(Atom a, Atom b)
    {
        var dx = a.PositionX - b.PositionX;
        var dy = a.PositionY - b.PositionY;
        var distance = Math.Sqrt(dx * dx + dy * dy);

        if (distance >= MaxDistance)
            return 0.0;

        return 1.0 - (distance / MaxDistance);
    }
}
