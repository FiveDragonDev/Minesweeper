using System.Drawing;

namespace Minesweeper.Engine.Utils
{
    public static class ColorUtils
    {
        public static uint ColorLerp(uint from, uint to, float phase) => (uint)ColorLerp(Color.FromArgb((int)from), Color.FromArgb((int)to), phase).ToArgb();
        public static int ColorLerp(int from, int to, float phase) => ColorLerp(Color.FromArgb(from), Color.FromArgb(to), phase).ToArgb();
        public static Color ColorLerp(Color from, Color to, float phase)
        {
            byte r = MathUtils.Lerp(from.R, to.R, phase);
            byte g = MathUtils.Lerp(from.G, to.G, phase);
            byte b = MathUtils.Lerp(from.B, to.B, phase);
            byte a = MathUtils.Lerp(from.A, to.A, phase);
            return Color.FromArgb(a, r, g, b);
        }
    }
}
