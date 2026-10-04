using Minesweeper.Game.World;

namespace Minesweeper.Game.Chunks
{
    public static class ChunkCoords
    {
        public static WorldPos ToWorld(ChunkPos c) => new(c.X << ChunkTerrain.SizeShift, c.Y << ChunkTerrain.SizeShift);
        public static WorldPos ToWorld(ChunkPos c, LocalPos l) => new((c.X << ChunkTerrain.SizeShift) + l.X, (c.Y << ChunkTerrain.SizeShift) + l.Y);

        public static ChunkPos ToChunk(WorldPos p) => new(p.X >> ChunkTerrain.SizeShift, p.Y >> ChunkTerrain.SizeShift);
        public static LocalPos ToLocal(WorldPos p) => new(p.X & ChunkTerrain.SizeMask, p.Y & ChunkTerrain.SizeMask);
    }
}
