using Minesweeper.Game;
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

        public void Render(IGameView gameView)
        {
            var world = gameView.World;
            var cursor = gameView.Cursor;

            System.Console.SetCursorPosition(0, 0);

            for (int i = 0; i < HEIGHT; i++)
            {
                int rowStart = i * (WIDTH * 2 + 1);
                _buffer.AsSpan(rowStart, WIDTH * 2).Fill(' ');

                for (int j = 0; j < WIDTH; j++)
                {
                    (int X, int Y) normalized = (j - (WIDTH >> 1), i - (HEIGHT >> 1));
                    (int X, int Y) = (normalized.X + cursor.Cell.X, normalized.Y + cursor.Cell.Y);
                    WorldPos p = new(X, Y);

                    int writePos = rowStart + j * 2;

                    var cell = world.GetCell(p);

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

            var cp = ChunkCoords.ToChunk(cursor.Cell);
            var lp = ChunkCoords.ToLocal(cursor.Cell);

            $"C: {cp.X} {cp.Y}".CopyTo(_buffer.AsSpan(1));
            $"L: {lp.X} {lp.Y}".CopyTo(_buffer.AsSpan((WIDTH * 2) + 1));
            _buffer.AsSpan(HEIGHT / 2 * ((WIDTH * 2) + 1) + WIDTH, 1)[0] = 'x';

            System.Console.Write(_buffer);
        }
    }
}
