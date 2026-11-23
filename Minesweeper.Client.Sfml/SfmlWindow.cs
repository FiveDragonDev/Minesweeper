using SFML.Graphics;
using SFML.System;

namespace Minesweeper.Client.Sfml
{
    public sealed class SfmlWindow : IDisposable
    {
        public event Action? Closed;
        public event Action<Vector2u>? Resized;

        internal uint Frame => _frame;

        private uint _frame = 0;

        private readonly RenderWindow _window;

        public SfmlWindow(int width, int height, string title)
        {
            _window = new(new(new((uint)width, (uint)height)), title);
            _window.SetView(new(new(0, 0), new(width, height)));

            _window.SetKeyRepeatEnabled(false);

            _window.Closed += (_, _) => Closed?.Invoke();
            _window.Resized += (_, e) =>
            {
                _window.SetView(new(new(0, 0), new(e.Size.X, e.Size.Y)));
                Resized?.Invoke(e.Size);
            };
        }

        public void RequestClose()
        {
            _window.Close();
            Closed?.Invoke();
        }

        public void PollEvents()
        {
            _frame++;
            _window.DispatchEvents();
        }

        public RenderWindow Raw() => _window;

        public void Dispose() => _window.Dispose();
    }
}
