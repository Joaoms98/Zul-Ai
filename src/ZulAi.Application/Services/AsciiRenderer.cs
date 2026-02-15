using ZulAi.Application.Interfaces;
using ZulAi.Domain.Entities;

namespace ZulAi.Application.Services;

public class AsciiRenderer : IAsciiRenderer
{
    public string Render(int width, int height, IReadOnlyList<Atom> atoms, IReadOnlyList<AtomConnection> connections)
    {
        var grid = new char[height, width];

        // Fill with spaces
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                grid[y, x] = ' ';

        // Draw border
        for (int x = 0; x < width; x++)
        {
            grid[0, x] = '-';
            grid[height - 1, x] = '-';
        }
        for (int y = 0; y < height; y++)
        {
            grid[y, 0] = '|';
            grid[y, width - 1] = '|';
        }
        grid[0, 0] = '+';
        grid[0, width - 1] = '+';
        grid[height - 1, 0] = '+';
        grid[height - 1, width - 1] = '+';

        // Draw connections using Bresenham's line algorithm
        var atomMap = atoms.Where(a => a.IsAlive).ToDictionary(a => a.Id);
        foreach (var conn in connections.Where(c => c.IsActive))
        {
            if (!atomMap.TryGetValue(conn.SourceAtomId, out var source) ||
                !atomMap.TryGetValue(conn.TargetAtomId, out var target))
                continue;

            var lineChar = conn.Strength switch
            {
                > 0.8 => '=',
                > 0.5 => '-',
                > 0.2 => ':',
                _ => '.'
            };

            DrawLine(grid, width, height,
                (int)Math.Round(source.PositionX), (int)Math.Round(source.PositionY),
                (int)Math.Round(target.PositionX), (int)Math.Round(target.PositionY),
                lineChar);
        }

        // Draw atoms on top of connections
        foreach (var atom in atoms.Where(a => a.IsAlive))
        {
            int x = (int)Math.Round(atom.PositionX);
            int y = (int)Math.Round(atom.PositionY);

            if (x > 0 && x < width - 1 && y > 0 && y < height - 1)
                grid[y, x] = atom.Symbol;
        }

        // Convert grid to string
        var lines = new List<string>(height);
        for (int y = 0; y < height; y++)
        {
            var chars = new char[width];
            for (int x = 0; x < width; x++)
                chars[x] = grid[y, x];
            lines.Add(new string(chars));
        }

        // Status line
        var aliveCount = atoms.Count(a => a.IsAlive);
        var activeConns = connections.Count(c => c.IsActive);
        var totalEnergy = atoms.Where(a => a.IsAlive).Sum(a => a.Energy);
        lines.Add($" Atoms: {aliveCount} | Connections: {activeConns} | Energy: {totalEnergy:F1}");

        return string.Join('\n', lines);
    }

    private static void DrawLine(char[,] grid, int gridWidth, int gridHeight,
        int x0, int y0, int x1, int y1, char ch)
    {
        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            if (x0 > 0 && x0 < gridWidth - 1 && y0 > 0 && y0 < gridHeight - 1)
            {
                if (grid[y0, x0] == ' ')
                    grid[y0, x0] = ch;
            }

            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }
    }
}
