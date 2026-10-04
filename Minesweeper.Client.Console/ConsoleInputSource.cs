using Minesweeper.Input.Abstractions;
using System.Numerics;

namespace Minesweeper.Client.Console
{
    public sealed class ConsoleInputSource : IInputSource
    {
        private Vector2 _moveInput = new(0, 0);
        private readonly List<InputAction> _inputActions = [];

        public Vector2 GetMoveAxis()
        {
            Vector2 moveInput = new(_moveInput.X, _moveInput.Y);
            _moveInput = new(0, 0);
            return moveInput;
        }

        public IEnumerable<InputAction> PollActions()
        {
            var actions = _inputActions.ToArray();
            _inputActions.Clear();
            return actions;
        }

        public void Update(float dt)
        {
            while (System.Console.KeyAvailable)
            {
                var key = System.Console.ReadKey(true).Key;

                var action = key switch
                {
                    ConsoleKey.E => InputAction.Reveal,
                    ConsoleKey.Q => InputAction.Flag,
                    _ => null as InputAction?,
                };

                if (action is { } a) _inputActions.Add(a);

                int x = 0, y = 0;

                if (key is ConsoleKey.LeftArrow or ConsoleKey.A) x -= 1;
                if (key is ConsoleKey.RightArrow or ConsoleKey.D) x += 1;

                if (key is ConsoleKey.UpArrow or ConsoleKey.W) y += 1;
                if (key is ConsoleKey.DownArrow or ConsoleKey.S) y -= 1;

                if ((x | y) != 0)
                {
                    _moveInput.X += x;
                    _moveInput.Y += y;
                }
            }
        }
    }
}
