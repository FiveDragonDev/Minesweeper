using Minesweeper.Engine.RNG;
using Minesweeper.Game.World;
using System.Runtime.CompilerServices;

namespace Minesweeper.Game.Chunks
{
    public static class ChunkSeed
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong From(ulong worldSeed, ChunkPos cp)
        {
            ulong packed = cp.Pack();

            ulong h = SplitMix64.Hash(worldSeed ^ 0xC0761D4188BD642FUL);
            h = SplitMix64.Hash(h ^ packed);
            return h;
        }
    }
}
