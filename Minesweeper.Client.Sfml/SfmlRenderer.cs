using Minesweeper.Engine.Utils;
using Minesweeper.Game;
using Minesweeper.Game.Chunks;
using Minesweeper.Game.World;
using Minesweeper.Rendering.Abstractions;
using SFML.Graphics;
using SFML.System;

namespace Minesweeper.Client.Sfml
{
    public sealed class SfmlRenderer : IRenderer
    {
        private const int CellSize = 32;
        private const float CellSizeF = CellSize;

        private const int AtlasWhiteSlot = 0;
        private const int AtlasCellTexturesOffset = 1;
        private const int AtlasSlotCount = AtlasCellTexturesOffset + 11; // 11 textures

        public float ViewRadius
        {
            get => _viewRadius;
            set
            {
                if (_viewRadius == value) return;
                _viewRadius = value;
                _fadeColorCache.Clear();
            }
        }
        private float _viewRadius = 7.5f;

        private readonly Dictionary<(int dx, int dy), Color> _fadeColorCache = [];

        private readonly Texture _atlas;

        private readonly CircleShape _player = new(CellSizeF / 10f, 32)
        {
            Origin = new(CellSizeF / 10f, CellSizeF / 10f),
            FillColor = Color.White,
        };

        private readonly Text _uiText = new(Assets.Font)
        {
            CharacterSize = 24,
            FillColor = Assets.TextColor,
        };

        private readonly VertexArray _cells;

        private readonly SfmlWindow _window;
        private readonly RenderWindow _raw;
        private WorldPos _uiCell = new(int.MinValue, int.MinValue);

        public SfmlRenderer(SfmlWindow window)
        {
            _window = window;
            _raw = window.Raw();
            _atlas = BuildAtlas();

            CalculateRowsAndCols(out int cols, out int rows);
            _cells = new(PrimitiveType.Triangles, (uint)(cols * rows * 6));

            _window.Resized += s =>
            {
                CalculateRowsAndCols(out int cols, out int rows);
                _cells.Resize((uint)(cols * rows * 6));
            };

            void CalculateRowsAndCols(out int cols, out int rows)
            {
                var halfWidth = ((int)_raw.Size.X / CellSize >> 1) + 1;
                var halfHeight = ((int)_raw.Size.Y / CellSize >> 1) + 1;

                cols = halfWidth * 2 + 1;
                rows = halfHeight * 2 + 1;
            }
        }

        private static Texture BuildAtlas()
        {
            using RenderTexture rt = new(new(AtlasSlotCount * CellSize, CellSize));
            rt.Clear(Color.Transparent);

            RectangleShape white = new(new Vector2f(CellSize, CellSize))
            {
                Position = new(AtlasWhiteSlot * CellSize, 0),
                FillColor = Color.White,
            };
            rt.Draw(white);

            for (int i = 0; i < Assets.CellTextures.Length; i++)
            {
                var sprite = new Sprite(Assets.CellTextures[i])
                {
                    Position = new((AtlasCellTexturesOffset + i) * CellSize, 0),
                    Scale = new(2, 2),
                };
                rt.Draw(sprite);
            }

            rt.Display();

            return new(rt.Texture) { Smooth = false };
        }

        public void Render(IGameView gameView)
        {
            var world = gameView.World;
            var cursor = gameView.Cursor;
            var cursorCell = cursor.Cell;

            _raw.Clear(Assets.BackgroundColor);

            var fracX = cursor.Position.X - MathF.Floor(cursor.Position.X);
            var fracY = cursor.Position.Y - MathF.Floor(cursor.Position.Y);

            var halfWidth = ((int)_raw.Size.X / CellSize >> 1) + 1;
            var halfHeight = ((int)_raw.Size.Y / CellSize >> 1) + 1;

            var sqViewRadius = _viewRadius * _viewRadius;

            uint vi = 0;

            for (int dy = -halfHeight; dy <= halfHeight; dy++)
            {
                for (int dx = -halfWidth; dx <= halfWidth; dx++)
                {
                    AppendCell(_cells, ref vi, world, cursorCell, dx, dy, fracX, fracY, sqViewRadius);
                }
            }

            _raw.Draw(_cells, new(_atlas));

            _player.Position = new(-fracX * CellSizeF, -fracY * CellSizeF);
            _raw.Draw(_player);

            DrawUi(cursorCell);

            _raw.Display();
        }

        private void AppendCell(VertexArray cells, ref uint vi, IWorldView world, WorldPos cursorCell, int dx, int dy, float fracX, float fracY, float sqViewRadius)
        {
            var sqLen = dx * dx + dy * dy;
            var outsideView = sqLen > sqViewRadius;

            WorldPos cellPos = new(cursorCell.X + dx, cursorCell.Y + dy);
            var view = world.GetCell(cellPos);

            int atlasSlot;
            Color tint;

            if (outsideView || view.Visual == CellVisual.Hidden)
            {
                atlasSlot = AtlasWhiteSlot;
                tint = GetFadeColor(dx, dy, sqLen);
            }
            else
            {
                atlasSlot = AtlasCellTexturesOffset + view.Visual switch
                {
                    CellVisual.Revealed => view.Number == 0 ? Assets.EmptyTexPtr : Assets.NumsTexPtr + view.Number - 1,
                    CellVisual.Flagged => Assets.NumsTexPtr + 8,
                    CellVisual.Exploded => Assets.NumsTexPtr + 9,
                    _ => -1,
                };
                tint = Color.White;
            }

            float x0 = (dx - fracX) * CellSizeF - CellSizeF * 0.5f;
            float y0 = (dy - fracY) * CellSizeF - CellSizeF * 0.5f;
            float x1 = x0 + CellSizeF;
            float y1 = y0 + CellSizeF;

            float u0 = atlasSlot * CellSize;
            float u1 = u0 + CellSize;

            cells[vi++] = new(new(x0, y0), tint, new(u0, 0));
            cells[vi++] = new(new(x1, y0), tint, new(u1, 0));
            cells[vi++] = new(new(x1, y1), tint, new(u1, CellSize));
            cells[vi++] = new(new(x0, y0), tint, new(u0, 0));
            cells[vi++] = new(new(x1, y1), tint, new(u1, CellSize));
            cells[vi++] = new(new(x0, y1), tint, new(u0, CellSize));
        }

        private Color GetFadeColor(int dx, int dy, float sqLen)
        {
            if (_fadeColorCache.TryGetValue((dx, dy), out var cached)) return cached;

            var phase = MathF.Max((MathF.Sqrt(sqLen) - _viewRadius) / 2f, 0f);
            Color color = new(ColorUtils.ColorLerp(Assets.BackgroundColor.ToInteger(), Assets.HiddenCellColor.ToInteger(), phase));

            _fadeColorCache[(dx, dy)] = color;
            return color;
        }

        private void DrawUi(WorldPos cursorCell)
        {
            if (cursorCell != _uiCell)
            {
                _uiCell = cursorCell;
                var cp = ChunkCoords.ToChunk(cursorCell);
                var lp = ChunkCoords.ToLocal(cursorCell);
                _uiText.DisplayedString = $"Chunk: ({cp.X}, {cp.Y})\nLocal: ({lp.X}, {lp.Y})";
                _uiText.Position = new(-_raw.Size.X / 2f + 10, -_raw.Size.Y / 2f + 10);
            }

            _raw.Draw(_uiText);
        }
    }
}
