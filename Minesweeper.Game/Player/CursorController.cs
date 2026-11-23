using Minesweeper.Game.World;
using System.Numerics;

namespace Minesweeper.Game.Player
{
    public sealed class CursorController(Cursor cursor, IWorldView world)
    {
        public ICursor Cursor => _cursor;

        public float Speed { get; set; } = 1;

        private Vector2 _position = cursor.Position;
        private readonly Cursor _cursor = cursor;

        public void Update(Vector2 axis, float dt)
        {
            if (axis.LengthSquared() < 1e-6f) return;
            var direction = Vector2.Normalize(axis);

            var target = _position + direction * Speed * dt;

            WorldPos next = new((int)target.X, (int)target.Y);
            if (!world.IsWalkable(next)) return;

            _position = target;
            _cursor.SetPosition(_position);
            _cursor.SetCell(next);
        }
    }
}
