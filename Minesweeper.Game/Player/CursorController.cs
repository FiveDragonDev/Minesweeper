using System.Numerics;

namespace Minesweeper.Game.Player
{
    public sealed class CursorController(Cursor cursor)
    {
        public ICursor Cursor => _cursor;

        public float Speed { get; set; } = 1;

        private readonly Cursor _cursor = cursor;

        public void Update(Vector2 axis, float dt)
        {
            if (axis.LengthSquared() < 1e-6f) return;
            var direction = Vector2.Normalize(axis);
            _cursor.SetPosition(_cursor.Position + direction * Speed * dt);
        }
    }
}
