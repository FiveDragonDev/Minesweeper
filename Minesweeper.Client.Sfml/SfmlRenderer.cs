using Minesweeper.Game.Chunks;
using Minesweeper.Game.World;
using Minesweeper.Engine.Utils;
using SFML.Graphics;
using SFML.System;
using Minesweeper.Rendering.Abstractions;
using Minesweeper.Game;

namespace Minesweeper.Client.Sfml
{
    public sealed class SfmlRenderer(SfmlWindow window) : IRenderer
    {
        private const int CellSize = 32;

        public float ViewRadius { get; set; } = 7.5f;

        private readonly RectangleShape _cell = new(new Vector2f(CellSize, CellSize))
        {
            Origin = new(CellSize / 2f, CellSize / 2f),
            FillColor = Color.White,
        };

        private readonly CircleShape _player = new(CellSize / 10f, 32)
        {
            Origin = new(CellSize / 10f, CellSize / 10f),
            FillColor = Color.White,
        };
        private readonly Text _uiText = new(Assets.Font)
        {
            CharacterSize = 24,
            FillColor = Assets.TextColor,
        };

        private readonly RenderWindow _window = window.Raw();

        public void Render(IGameView gameView)
        {
            var world = gameView.World;
            var cursor = gameView.Cursor;

            _window.Clear(Assets.BackgroundColor);

            var halfHeight = ((int)_window.Size.Y / CellSize >> 1) + 1;
            var halfWidth = ((int)_window.Size.X / CellSize >> 1) + 1;

            var truncatedX = cursor.Position.X >= 0 ? cursor.Position.X - MathF.Floor(cursor.Position.X) : cursor.Position.X - MathF.Ceiling(cursor.Position.X);
            var truncatedY = cursor.Position.Y >= 0 ? cursor.Position.Y - MathF.Floor(cursor.Position.Y) : cursor.Position.Y - MathF.Ceiling(cursor.Position.Y);

            for (int i = -halfHeight; i <= halfHeight; i++)
            {
                for (int j = -halfWidth; j <= halfWidth; j++)
                {
                    _cell.Position = new((j - truncatedX) * CellSize, (i - truncatedY) * CellSize);
                    _cell.Texture = null;

                    var sqLen = i * i + j * j;
                    if (sqLen > ViewRadius * ViewRadius)
                    {
                        var len = MathF.Sqrt(sqLen);
                        var phase = (len - ViewRadius) / 2f;
                        _cell.FillColor = new(ColorUtils.ColorLerp(Assets.BackgroundColor.ToInteger(), Assets.HiddenCellColor.ToInteger(), phase));
                        _window.Draw(_cell);
                        _cell.FillColor = Color.White;
                        continue;
                    }

                    WorldPos cellPos = new(j + cursor.Cell.X, i + cursor.Cell.Y);
                    var cellView = world.GetCell(cellPos);

                    switch (cellView.Visual)
                    {
                        case CellVisual.Revealed:
                            _cell.Texture = cellView.Number == 0 ? Assets.GetEmptyTex(world.GetSeed(), cellPos) : Assets.GetTexByNumber(cellView.Number);
                            break;

                        case CellVisual.Flagged:
                            _cell.Texture = Assets.GetFlagTex();
                            break;

                        case CellVisual.Exploded:
                            _cell.Texture = Assets.GetMineTex();
                            break;

                        case CellVisual.Hidden:
                        default:
                            break;
                    }

                    if (cellView.Visual != CellVisual.Hidden) _window.Draw(_cell);
                }
            }

            _player.Position = new(-truncatedX * CellSize, -truncatedY * CellSize);
            _window.Draw(_player);

            var cp = ChunkCoords.ToChunk(cursor.Cell);
            var lp = ChunkCoords.ToLocal(cursor.Cell);
            _uiText.DisplayedString = $"Chunk: ({cp.X}, {cp.Y})\nLocal: ({lp.X}, {lp.Y})";
            var bounds = _uiText.GetLocalBounds();
            _uiText.Origin = new(-bounds.Left - 10, bounds.Top - 10);
            _uiText.Position = new(-_window.Size.X / 2f, -_window.Size.Y / 2f);
            _window.Draw(_uiText);

            _window.Display();
        }
    }
}
