using Minesweeper.Engine.Utils;
using Minesweeper.Game.Save;
using Minesweeper.Game.World;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Minesweeper.Game.Chunks
{
    public sealed class ChunkState
    {
        public bool IsEmpty
        {
            get
            {
                for (int i = 0; i < ChunkTerrain.Size; i++)
                    if ((_revealed[i] | _flags[i]) != 0) return false;

                return true;
            }
        }

        private readonly uint[] _revealed = new uint[ChunkTerrain.Size];
        private readonly uint[] _flags = new uint[ChunkTerrain.Size];
        private readonly uint[] _queued = new uint[ChunkTerrain.Size];

        private readonly ulong[] _numbers = new ulong[ChunkTerrain.Size * (ChunkTerrain.Size >> ChunkTerrain.SizeShift - 1)];

        public ChunkState() { }
        private ChunkState(uint[] revealed, uint[] flags, uint[] queued, ulong[] numbers)
        {
            _revealed = revealed;
            _flags = flags;
            _queued = queued;
            _numbers = numbers;
        }

        public byte GetNumber(LocalPos l)
        {
            int bitIndex = l.Y * ChunkTerrain.Size + l.X;
            int word = bitIndex >> ChunkTerrain.SizeShift - 1;
            int shift = (bitIndex & ((1 << ChunkTerrain.SizeShift - 1) - 1)) << 2;
            return (byte)((_numbers[word] >> shift) & 0xF);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsRevealed(LocalPos l) => ((_revealed[l.Y] >> l.X) & 1u) != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsFlagged(LocalPos l) => ((_flags[l.Y] >> l.X) & 1u) != 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsQueued(LocalPos l) => ((_queued[l.Y] >> l.X) & 1u) != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reveal(LocalPos l, byte number)
        {
            _revealed[l.Y] |= 1u << l.X;

            int bitIndex = l.Y * ChunkTerrain.Size + l.X;
            int word = bitIndex >> ChunkTerrain.SizeShift - 1;
            int shift = (bitIndex & ((1 << ChunkTerrain.SizeShift - 1) - 1)) << 2;

            _numbers[word] = (_numbers[word] & ~(0xFUL << shift)) | ((ulong)number << shift);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ToggleFlag(LocalPos l)
        {
            ref var row = ref _flags[l.Y];
            var shift = 1u << l.X;
            row ^= shift;
            return (row & shift) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void MarkQueued(LocalPos l) => _queued[l.Y] |= 1u << l.X;

        internal ChunkSave ToSave() => new()
        {
            Revealed = SerializationUtils.ToBase64(_revealed),
            Flagged = SerializationUtils.ToBase64(_flags),
            Queued = SerializationUtils.ToBase64(_queued),
            Numbers = SerializationUtils.ToBase64(_numbers),
        };
        internal static ChunkState FromSave(ChunkSave s) => new(
            SerializationUtils.FromBase64<uint>(s.Revealed), SerializationUtils.FromBase64<uint>(s.Flagged),
            SerializationUtils.FromBase64<uint>(s.Queued), SerializationUtils.FromBase64<ulong>(s.Numbers));

    }
}
