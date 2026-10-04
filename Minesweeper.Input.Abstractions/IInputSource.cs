using System.Numerics;

namespace Minesweeper.Input.Abstractions
{
    public interface IInputSource
    {
        IEnumerable<InputAction> PollActions();
        Vector2 GetMoveAxis();

        void Update();
    }
}
