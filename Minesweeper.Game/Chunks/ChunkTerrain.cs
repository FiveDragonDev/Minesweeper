using Minesweeper.Game.World;
using System.Runtime.CompilerServices;

namespace Minesweeper.Game.Chunks
{
    public sealed class ChunkTerrain
    {
        public const int Size = 1 << SizeShift;
        public const int SizeMask = Size - 1;
        public const int SizeShift = 5;

        private readonly uint[] _mines;

        public ChunkTerrain(uint[] mines) => _mines = mines;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsMine(LocalPos l) => ((_mines[l.Y] >> l.X) & 1u) != 0;
    }
}
