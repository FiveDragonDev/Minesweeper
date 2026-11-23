using Minesweeper.Game.Chunks;

namespace Minesweeper.Game.World
{
    public enum CellVisual : byte { Hidden, Revealed, Flagged, Exploded, }

    public readonly record struct CellView(byte Number, CellVisual Visual);

    public interface IWorldView
    {
        CellView GetCell(WorldPos p);
        int Version(ChunkPos p);

        ulong GetSeed();

        bool IsWalkable(WorldPos p);
    }
    public sealed partial class World : IWorldView
    {
        public CellView GetCell(WorldPos p)
        {
            var cp = ChunkCoords.ToChunk(p);
            var lp = ChunkCoords.ToLocal(p);
            if (!_chunks.TryGetValue(cp, out var chunk)) return new CellView(0, CellVisual.Hidden);

            if (chunk.IsFlagged(lp)) return new(0, CellVisual.Flagged);

            if (chunk.IsRevealed(lp))
            {
                if (IsMine(p)) return new(0, CellVisual.Exploded);
                else return new(chunk.GetNumber(lp), CellVisual.Revealed);
            }

            return new(0, CellVisual.Hidden);
        }

        public int Version(ChunkPos p) => -1;

        public ulong GetSeed() => _seed;

        public bool IsWalkable(WorldPos p)
        {
            var cp = ChunkCoords.ToChunk(p);
            if (_chunks.TryGetValue(cp, out var chunk))
            {
                var lp = ChunkCoords.ToLocal(p);
                if (chunk.IsRevealed(lp)) return true;
            }

            for (int i = 0; i < Neighbors.Length; i++)
            {
                WorldPos np = new(p.X + Neighbors[i].DX, p.Y + Neighbors[i].DY);
                if (IsRevealed(np)) return true;
            }

            return false;
        }
    }
}
