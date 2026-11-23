using Minesweeper.Game.World;
using System.Numerics;

namespace Minesweeper.Game.Player
{
    public interface ICursor
    {
        WorldPos Cell { get; }
        Vector2 Position { get; }
    }

    public sealed class Cursor(WorldPos start) : ICursor
    {
        public WorldPos Cell { get; private set; } = start;
        public Vector2 Position { get; private set; } = new(start.X, start.Y);

        internal void SetCell(WorldPos cell) => Cell = cell;
        internal void SetPosition(Vector2 position) => Position = position;
    }
}
