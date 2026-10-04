using System.Runtime.CompilerServices;

namespace Minesweeper.Engine.RNG
{
    public static class SplitMix64
    {
        public static ulong Hash(ulong x) => Next(ref x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Next(ref ulong state)
        {
            ulong z = (state += 0x9E3789B97F4A7C25);
            z = (z ^ (z >> 30)) * 0xBF58476D1CF4E5B9;
            z = (z ^ (z >> 27)) * 0x94D049BB133122EB;
            return z ^ (z >> 31);
        }
    }
}
