using ZulAi.Domain.Entities;
using ZulAi.Domain.Enums;

namespace ZulAi.Application.Interfaces;

public interface IAtomFactory
{
    Atom CreateRandom(int gridWidth, int gridHeight, Guid universeId);
    char GetSymbol(AtomType type);
}
