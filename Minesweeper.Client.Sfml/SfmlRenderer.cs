using Minesweeper.Game.Chunks;
using Minesweeper.Game.World;
using Minesweeper.Engine.Utils;
using SFML.Graphics;
using SFML.System;
using Minesweeper.Rendering.Abstractions;
using System.Numerics;

namespace Minesweeper.Client.Sfml
{
    public sealed class SfmlRenderer(SfmlWindow window) : IRenderer
    {
        private const int CellSize = 32;

        public float ViewRadius { get; set; } = 7.5f;

        private WorldPos _cellPosition = new(0, 0);
        private Vector2 _position = new(0, 0);

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

        public void SetPosition(Vector2 p, WorldPos c)
        {
            _position = p;
            _cellPosition = c;
        }

        public void Render(IWorldView worldView)
        {
            _window.Clear(Assets.BackgroundColor);

            var halfHeight = ((int)_window.Size.Y / CellSize >> 1) + 1;
            var halfWidth = ((int)_window.Size.X / CellSize >> 1) + 1;

            var truncatedX = _position.X >= 0 ? _position.X - MathF.Floor(_position.X) : _position.X - MathF.Ceiling(_position.X);
            var truncatedY = _position.Y >= 0 ? _position.Y - MathF.Floor(_position.Y) : _position.Y - MathF.Ceiling(_position.Y);

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

                    WorldPos cellPos = new(j + _cellPosition.X, i + _cellPosition.Y);
                    var cellView = worldView.GetCell(cellPos);

                    switch (cellView.Visual)
                    {
                        case CellVisual.Revealed:
                            _cell.Texture = cellView.Number == 0 ? Assets.GetEmptyTex(worldView.GetSeed(), cellPos) : Assets.GetTexByNumber(cellView.Number);
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

            var cp = ChunkCoords.ToChunk(_cellPosition);
            var lp = ChunkCoords.ToLocal(_cellPosition);
            _uiText.DisplayedString = $"Chunk: ({cp.X}, {cp.Y})\nLocal: ({lp.X}, {lp.Y})";
            var bounds = _uiText.GetLocalBounds();
            _uiText.Origin = new(-bounds.Left - 10, bounds.Top - 10);
            _uiText.Position = new(-_window.Size.X / 2f, -_window.Size.Y / 2f);
            _window.Draw(_uiText);

            _window.Display();
        }
    }
}
