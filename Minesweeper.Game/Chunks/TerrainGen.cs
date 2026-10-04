using Minesweeper.Engine.RNG;
using Minesweeper.Game.World;

namespace Minesweeper.Game.Chunks
{
    public static class TerrainGen
    {
        public static uint[] GenerateWavePattern(ulong worldSeed, ChunkPos position, int minePercent, int frequency)
        {
            Xoshiro256PP random = new(ChunkSeed.From(worldSeed, position));

            const float angle = 45 * (MathF.PI / 180f);

            var mines = new uint[ChunkTerrain.Size];
            var mul = random.NextBounded(100) / 50f + 0.5f;
            for (int y = 0; y < ChunkTerrain.Size; y++)
            {
                uint row = 0;
                for (int x = 0; x < ChunkTerrain.Size; x++)
                {
                    float u = x * mul * MathF.Cos(angle) + y * mul * MathF.Sin(angle);
                    float wave = MathF.Sin(u * frequency * (2.0f * MathF.PI / ChunkTerrain.Size));

                    if (wave + 1 < minePercent / 50f) row |= 1u << x;
                }
                mines[y] = row;
            }

            return mines;
        }

        public static uint[] GenerateRandom(ulong worldSeed, ChunkPos position, int minePercent, bool safeOrigin = false)
        {
            Xoshiro256PP random = new(ChunkSeed.From(worldSeed, position));

            var mines = new uint[ChunkTerrain.Size];
            for (int y = 0; y < ChunkTerrain.Size; y++)
            {
                uint row = 0;
                for (int x = 0; x < ChunkTerrain.Size; x++)
                {
                    if (random.NextBounded(100) < (uint)minePercent) row |= 1u << x;
                }
                mines[y] = row;
            }

            if (safeOrigin)
            {
                var halfSize = ChunkTerrain.Size >> 1;
                for (int i = -1; i <= 1; i++)
                {
                    ref var row = ref mines[halfSize + i];
                    for (int j = -1; j <= 1; j++)
                    {
                        row &= ~(1u << (halfSize + j));
                    }
                }
            }

            return mines;
        }
    }
}
