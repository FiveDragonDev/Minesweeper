namespace Minesweeper.Game.World
{
    public readonly record struct ChunkPos(int X, int Y)
    {
        public static ChunkPos FromPacked(ulong packed) => new((int)((packed >> 32) & 0xFFFFFFFF), (int)(packed & 0xFFFFFFFF));
        public ulong Pack() => ((ulong)(uint)X << 32) | (uint)Y;
    }
    public readonly record struct LocalPos(int X, int Y)
    {
        public static LocalPos FromPacked(ulong packed) => new((int)((packed >> 32) & 0xFFFFFFFF), (int)(packed & 0xFFFFFFFF));
        public ulong Pack() => ((ulong)(uint)X << 32) | (uint)Y;
    }
    public readonly record struct WorldPos(int X, int Y)
    {
        public static WorldPos FromPacked(ulong packed) => new((int)((packed >> 32) & 0xFFFFFFFF), (int)(packed & 0xFFFFFFFF));
        public ulong Pack() => ((ulong)(uint)X << 32) | (uint)Y;
    }
}
