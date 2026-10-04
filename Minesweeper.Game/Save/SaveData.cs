namespace Minesweeper.Game.Save
{
    public sealed class SaveData
    {
        public int Version { get; set; } = 1;
        public ulong Seed { get; set; }
        public int MinePercent { get; set; }
        public int CursorX { get; set; }
        public int CursorY { get; set; }

        public Dictionary<string, ChunkSave> Chunks { get; set; } = [];

        // public List<string> Frontier { get; set; } = [];
    }

    public sealed class ChunkSave
    {
        public string Revealed { get; set; } = "";
        public string Flagged { get; set; } = "";
        public string Queued { get; set; } = "";
        public string Numbers { get; set; } = "";
    }
}
