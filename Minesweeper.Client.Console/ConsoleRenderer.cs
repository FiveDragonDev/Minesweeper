using Minesweeper.Game.Chunks;
using Minesweeper.Game.World;
using Minesweeper.Rendering.Abstractions;

namespace Minesweeper.Client.Console
{
    public sealed class ConsoleRenderer : IRenderer
    {
        public const int WIDTH = 60;
        public const int HEIGHT = 60;

        private readonly char[] _buffer = new char[WIDTH * HEIGHT * 2 + HEIGHT - 1];

        private WorldPos _position;
        
        public ConsoleRenderer()
        {
            _buffer.AsSpan().Fill(' ');

            for (int i = 0; i < HEIGHT - 1; i++)
                _buffer[i * ((WIDTH * 2) + 1) + WIDTH * 2] = '\n';

            if (OperatingSystem.IsWindows())
            {
                System.Console.SetWindowSize(WIDTH * 2, HEIGHT);
                System.Console.SetBufferSize(WIDTH * 2, HEIGHT);
            }
        }

        public void SetPosition(WorldPos p) => _position = p;

        public void Render(IWorldView worldView)
        {
            System.Console.SetCursorPosition(0, 0);

            for (int i = 0; i < HEIGHT; i++)
            {
                int rowStart = i * (WIDTH * 2 + 1);
                _buffer.AsSpan(rowStart, WIDTH * 2).Fill(' ');

                for (int j = 0; j < WIDTH; j++)
                {
                    (int X, int Y) normalized = (j - (WIDTH >> 1), i - (HEIGHT >> 1));
                    (int X, int Y) = (normalized.X + _position.X, normalized.Y + _position.Y);
                    WorldPos p = new(X, Y);

                    int writePos = rowStart + j * 2;

                    var cell = worldView.GetCell(p);

                    switch (cell.Visual)
                    {
                        case CellVisual.Hidden:
                            break;

                        case CellVisual.Revealed:
                            _buffer[writePos] = cell.Number.ToString()[0];
                            break;

                        case CellVisual.Flagged:
                            _buffer[writePos] = '>';
                            break;

                        case CellVisual.Exploded:
                            _buffer[writePos] = '*';
                            break;

                        default:
                            break;
                    }
                }
            }

            var currentPosition = ChunkCoords.ToChunk(_position);

            const string TestText = "DEMO";
            TestText.CopyTo(_buffer.AsSpan());
            $"{currentPosition.X} {currentPosition.Y}".CopyTo(_buffer.AsSpan((WIDTH * 2) + 1));
            _buffer.AsSpan(HEIGHT / 2 * ((WIDTH * 2) + 1) + WIDTH, 1)[0] = 'x';

            System.Console.Write(_buffer);
        }
    }
}
