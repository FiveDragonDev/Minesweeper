using Minesweeper.Game.Chunks;

namespace Minesweeper.Game.World
{
    public sealed partial class World
    {
        private static readonly (int DX, int DY)[] Neighbors = [(-1, 1), (0, 1), (1, 1), (-1, 0), (1, 0), (-1, -1), (0, -1), (1, -1)];

        private readonly Queue<WorldPos> _frontier = [];

        private readonly ulong _seed;
        private readonly int _minePercent;

        private readonly Dictionary<ChunkPos, ChunkState> _chunks = [];
        private readonly Dictionary<ChunkPos, ChunkTerrain> _terrain = [];

        public World(ulong seed, int minePercent)
        {
            _seed = seed;
            _minePercent = minePercent;
        }

        public void Update(int cellBudget)
        {
            if (_frontier.Count > 0)
            {
                int processed = 0;

                while (processed < cellBudget && _frontier.TryDequeue(out var p))
                {
                    var lp = ChunkCoords.ToLocal(p);
                    var chunk = GetOrCreateChunkAt(ChunkCoords.ToChunk(p));
                    if (IsMine(p)) continue;

                    byte number = 0;
                    for (int i = 0; i < Neighbors.Length; i++)
                        if (IsMine(new(p.X + Neighbors[i].DX, p.Y + Neighbors[i].DY))) number++;

                    chunk.Reveal(lp, number);
                    CellRevealed?.Invoke(p, number);
                    processed++;

                    if (number == 0)
                        for (int i = 0; i < Neighbors.Length; i++)
                            TryEnqueue(new(p.X + Neighbors[i].DX, p.Y + Neighbors[i].DY));
                }
            }
        }
        public void UnloadFarChunks(ChunkPos center, int radius)
        {
            for (int i = _terrain.Count - 1; i >= 0; i--)
            {
                var p = _terrain.ElementAt(i).Key;
                if (Math.Abs(p.X - center.X) > radius ||
                    Math.Abs(p.Y - center.Y) > radius)
                    _terrain.Remove(p);
            }

            for (int i = _chunks.Count - 1; i >= 0; i--)
            {
                var p = _chunks.ElementAt(i).Key;
                if ((Math.Abs(p.X - center.X) > radius ||
                    Math.Abs(p.Y - center.Y) > radius) &&
                    _chunks[p].IsEmpty)
                    _chunks.Remove(p);
            }
        }

        private bool TryEnqueue(WorldPos p)
        {
            var chunk = GetOrCreateChunkAt(ChunkCoords.ToChunk(p));
            var lp = ChunkCoords.ToLocal(p);

            if (chunk.IsRevealed(lp) || chunk.IsFlagged(lp) || chunk.IsQueued(lp)) return false;

            chunk.MarkQueued(lp);
            _frontier.Enqueue(p);
            return true;
        }

        private ChunkState GetOrCreateChunkAt(ChunkPos cp)
        {
            if (_chunks.TryGetValue(cp, out var c)) return c;
            c = new ChunkState();
            _chunks[cp] = c;
            return c;
        }
        private ChunkTerrain TerrainAt(ChunkPos cp)
        {
            if (_terrain.TryGetValue(cp, out var t)) return t;
            bool safeOrigin = false;
            if ((cp.X | cp.Y) == 0) safeOrigin = true;
            t = new ChunkTerrain(TerrainGen.GenerateRandom(_seed, cp, _minePercent, safeOrigin));
            _terrain[cp] = t;
            return t;
        }

        private bool IsMine(WorldPos p) => TerrainAt(ChunkCoords.ToChunk(p)).IsMine(ChunkCoords.ToLocal(p));
        private bool IsRevealed(WorldPos p) => _chunks.TryGetValue(ChunkCoords.ToChunk(p), out var chunk) && chunk.IsRevealed(ChunkCoords.ToLocal(p));
        private bool IsFlagged(WorldPos p) => _chunks.TryGetValue(ChunkCoords.ToChunk(p), out var chunk) && chunk.IsFlagged(ChunkCoords.ToLocal(p));
    }
}
