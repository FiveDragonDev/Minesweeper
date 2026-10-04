namespace Minesweeper.Engine.RNG
{
    public struct Xoshiro256PP
    {
        private ulong _s0, _s1, _s2, _s3;

        public Xoshiro256PP(ulong seed)
        {
            _s0 = SplitMix64.Next(ref seed);
            _s1 = SplitMix64.Next(ref seed);
            _s2 = SplitMix64.Next(ref seed);
            _s3 = SplitMix64.Next(ref seed);

            if ((_s0 | _s1 | _s2 | _s3) == 0) _s0 = 1;
        }

        private static ulong Rotl(ulong x, int k) => (x << k) | (x >> (64 - k));

        public ulong NextUInt64()
        {
            var result = Rotl(_s0 + _s3, 23) + _s0;
            var t = _s1 << 17;

            _s2 ^= _s0;
            _s3 ^= _s1;
            _s1 ^= _s2;
            _s0 ^= _s3;

            _s2 ^= t;
            _s3 = Rotl(_s3, 45);

            return result;
        }

        public uint NextUInt32() => (uint)(NextUInt64() >> 32);
        public bool NextBool() => (NextUInt64() >> 63) != 0;
        public uint NextBounded(uint maxExclusive)
        {
            var m = (ulong)NextUInt32() * maxExclusive;
            var l = (uint)m;

            if (l < maxExclusive)
            {
                var t = (uint)(-(int)maxExclusive) % maxExclusive;
                while (l < t)
                {
                    m = (ulong)NextUInt32() * maxExclusive;
                    l = (uint)m;
                }
            }
            return (uint)(m >> 32);
        }
    }
}
