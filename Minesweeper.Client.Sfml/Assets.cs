using SFML.Graphics;
using Minesweeper.Game.World;

namespace Minesweeper.Client.Sfml
{
    public static class Assets
    {
        public const string AssetsPath = "Assets";

        public static readonly Color BackgroundColor = new(0x0d3532ff);
        public static readonly Color HiddenCellColor = new(0x082126ff);
        public static readonly Color TextColor = new(0xffae70ff);

        public const int EmptyTexPtr = 0;
        public const int EmptyTexsLen = 1;
        public const int NumsTexPtr = EmptyTexPtr + EmptyTexsLen;

        public static readonly Texture[] CellTextures = [
            new Texture(Path.Combine(AssetsPath, "0.png")),
            new Texture(Path.Combine(AssetsPath, "1.png")),
            new Texture(Path.Combine(AssetsPath, "2.png")),
            new Texture(Path.Combine(AssetsPath, "3.png")),
            new Texture(Path.Combine(AssetsPath, "4.png")),
            new Texture(Path.Combine(AssetsPath, "5.png")),
            new Texture(Path.Combine(AssetsPath, "6.png")),
            new Texture(Path.Combine(AssetsPath, "7.png")),
            new Texture(Path.Combine(AssetsPath, "8.png")),
            new Texture(Path.Combine(AssetsPath, "flag.png")),
            new Texture(Path.Combine(AssetsPath, "mine.png")),
            ];

        public static readonly Font Font = new(Path.Combine(AssetsPath, "font.ttf"));

        public static Texture GetEmptyTex(ulong seed, WorldPos p)
        {
            return CellTextures[EmptyTexPtr];
            /*var cp = PosUtils.ToChunk(p);
            var lp = PosUtils.ToLocal(p);
            var emptyTexs = CellTextures.AsSpan(EmptyTexPtr, EmptyTexsLen);
            Xoshiro256PP rng = new(ChunkSeed.From(seed, cp));
            for (int i = 0; i < lp.Y * ChunkTerrain.Size + lp.X - 1; i++)
            {
                _ = rng.NextBounded(EmptyTexsLen);
            }
            var randomIndex = (int)rng.NextBounded(EmptyTexsLen);
            return emptyTexs[randomIndex];*/
        }
        public static Texture GetTexByNumber(byte number)
        {
            if (number < 1 || number > 8) throw new ArgumentOutOfRangeException(nameof(number), "Number must be between 1 and 8.");
            return CellTextures[NumsTexPtr + number - 1];
        }

        public static Texture GetFlagTex() => CellTextures[NumsTexPtr + 8];
        public static Texture GetMineTex() => CellTextures[NumsTexPtr + 9];
    }
}
