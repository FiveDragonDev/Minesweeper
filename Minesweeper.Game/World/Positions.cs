namespace Minesweeper.Game.World
{
    public readonly record struct ChunkPos(int X, int Y)
    {
        public ulong Pack() => ((ulong)(uint)X << 32) | (uint)Y;
    }
    public readonly record struct LocalPos(int X, int Y)
    {
        public ulong Pack() => ((ulong)(uint)X << 32) | (uint)Y;
    }
    public readonly record struct WorldPos(int X, int Y)
    {
        public ulong Pack() => ((ulong)(uint)X << 32) | (uint)Y;
    }
}
