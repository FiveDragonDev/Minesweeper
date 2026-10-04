using System.Numerics;

namespace Minesweeper.Engine.Utils
{
    public static class MathUtils
    {
        public static byte Lerp(byte from, byte to, float phase) => (byte)Math.Clamp(Lerp((float)from, to, phase), 0, 255);
        public static float Lerp(float from, float to, float phase)
        {
            if (to < from)
            {
                (to, from) = (from, to);
                phase = 1 - phase;
            }

            return from + ((to - from) * phase);
        }

        public static Vector2 SmoothDamp(Vector2 current, Vector2 target, ref Vector2 velocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            smoothTime = MathF.Max(0.0001f, smoothTime);
            float omega = 2 / smoothTime;

            float x = omega * deltaTime;
            float exp = 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);

            Vector2 change = current - target;
            Vector2 originalTo = target;

            float maxChange = maxSpeed * smoothTime;
            change = ClampMagnitude(change, maxChange);
            target = current - change;

            Vector2 temp = (velocity + omega * change) * deltaTime;
            velocity = (velocity - omega * temp) * exp;
            Vector2 output = target + (change + temp) * exp;

            if (Vector2.Dot(originalTo - current, output - originalTo) > 0)
            {
                output = originalTo;
                velocity = (output - originalTo) / deltaTime;
            }

            return output;
        }

        public static Vector2 ClampMagnitude(Vector2 v, float max)
        {
            float sq = v.LengthSquared();
            if (sq > max * max && sq > 1e-12f) return v * (max / MathF.Sqrt(sq));
            return v;
        }
    }
}
