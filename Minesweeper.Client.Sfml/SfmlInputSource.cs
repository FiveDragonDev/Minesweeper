using Minesweeper.Input.Abstractions;
using SFML.Window;
using System.Collections;
using System.Numerics;

namespace Minesweeper.Client.Sfml
{
    public sealed class SfmlInputSource(SfmlWindow window) : IInputSource
    {
        public Vector2 MousePosition => _events.MousePosition;

        private Vector2 _moveInput = new(0, 0);
        private readonly List<InputAction> _inputActions = [];

        private readonly EventsListener _events = new(window);

        public Vector2 GetMoveAxis()
        {
            Vector2 moveInput = _moveInput;
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
            if (_events.IsKeyPressed(Keyboard.Key.Escape)) window.RequestClose();

            if (_events.IsKeyDown(Keyboard.Key.E)) _inputActions.Add(InputAction.Reveal);
            else if (_events.IsKeyPressed(Keyboard.Key.Q)) _inputActions.Add(InputAction.Flag);

            Vector2 input = new(0, 0);
            if (_events.IsKeyDown(Keyboard.Key.Left) || _events.IsKeyDown(Keyboard.Key.A)) input -= Vector2.UnitX;
            if (_events.IsKeyDown(Keyboard.Key.Right) || _events.IsKeyDown(Keyboard.Key.D)) input += Vector2.UnitX;

            if (_events.IsKeyDown(Keyboard.Key.Up) || _events.IsKeyDown(Keyboard.Key.W)) input -= Vector2.UnitY;
            if (_events.IsKeyDown(Keyboard.Key.Down) || _events.IsKeyDown(Keyboard.Key.S)) input += Vector2.UnitY;

            _moveInput += input * dt;
        }

        private sealed class EventsListener
        {
            public Vector2 MousePosition { get; private set; } = new(0, 0);

            private readonly BitArray _keys = new(1032);
            private readonly uint[] _frames = new uint[1032];

            private readonly SfmlWindow _window;

            public EventsListener(SfmlWindow window)
            {
                _window = window;

                _keys.SetAll(false);
                _frames.AsSpan().Clear();

                var raw = _window.Raw();

                raw.KeyPressed += HandleKeyPressed;
                raw.KeyReleased += HandleKeyReleased;

                raw.MouseButtonPressed += HandleMousePressed;
                raw.MouseButtonReleased += HandleMouseReleased;

                raw.MouseMoved += HandleMouseMoved;
            }

            public bool IsMouseButtonDown(Mouse.Button button)
            {
                int keycode = (int)button;
                if (keycode < 0 || keycode > 8) return false;
                return _keys.Get(keycode + 1024);
            }
            public bool IsMouseButtonPressed(Mouse.Button button)
            {
                int keycode = (int)button;
                if (keycode < 0 || keycode > 8) return false;
                return _keys.Get(keycode + 1024) && _frames[keycode + 1024] == _window.Frame;
            }

            public bool IsKeyDown(Keyboard.Key key)
            {
                int keycode = (int)key;
                if (keycode < 0 || keycode > 1024) return false;
                return _keys.Get(keycode);
            }
            public bool IsKeyPressed(Keyboard.Key key)
            {
                int keycode = (int)key;
                if (keycode < 0 || keycode > 1024) return false;
                return _keys.Get(keycode) && _frames[keycode] == _window.Frame;
            }

            private void HandleKeyPressed(object? sender, KeyEventArgs e)
            {
                var key = (int)e.Code;
                if (key < 0 || key > 1024) return;
                _keys.Set(key, true);
                _frames[key] = _window.Frame;
            }
            private void HandleKeyReleased(object? sender, KeyEventArgs e)
            {
                var key = (int)e.Code;
                if (key < 0 || key > 1024) return;
                _keys.Set(key, false);
            }

            private void HandleMousePressed(object? sender, MouseButtonEventArgs e)
            {
                var key = (int)e.Button;
                if (key < 0 || key > 8) return;
                _keys.Set(key + 1024, true);
                _frames[key + 1024] = _window.Frame;
            }
            private void HandleMouseReleased(object? sender, MouseButtonEventArgs e)
            {
                var key = (int)e.Button;
                if (key < 0 || key > 8) return;
                _keys.Set(key + 1024, false);
            }

            private void HandleMouseMoved(object? sender, MouseMoveEventArgs e) => MousePosition = new(e.Position.X, e.Position.Y);
        }
    }
}
