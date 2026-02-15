using ZulAi.Domain.Entities;

namespace ZulAi.Application.Interfaces;

public interface IAsciiRenderer
{
    string Render(int width, int height, IReadOnlyList<Atom> atoms, IReadOnlyList<AtomConnection> connections);
}
