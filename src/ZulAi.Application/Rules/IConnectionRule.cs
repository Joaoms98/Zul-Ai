using ZulAi.Domain.Entities;

namespace ZulAi.Application.Rules;

public interface IConnectionRule
{
    double Evaluate(Atom a, Atom b);
    double Weight { get; }
}
