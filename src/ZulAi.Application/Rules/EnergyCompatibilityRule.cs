using ZulAi.Domain.Entities;

namespace ZulAi.Application.Rules;

public class EnergyCompatibilityRule : IConnectionRule
{
    private const double SweetSpot = 30.0;
    private const double MaxDeviation = 50.0;

    public double Weight => 0.3;

    public double Evaluate(Atom a, Atom b)
    {
        var energyDiff = Math.Abs(a.Energy - b.Energy);
        var score = 1.0 - Math.Abs(energyDiff - SweetSpot) / MaxDeviation;
        return Math.Clamp(score, 0.0, 1.0);
    }
}
