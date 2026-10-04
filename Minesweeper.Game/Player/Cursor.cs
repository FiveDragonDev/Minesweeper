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
        public WorldPos Cell => new((int)Position.X, (int)Position.Y);
        public Vector2 Position { get; private set; } = new(start.X, start.Y);

        internal void SetPosition(Vector2 position) => Position = position;
    }
}
