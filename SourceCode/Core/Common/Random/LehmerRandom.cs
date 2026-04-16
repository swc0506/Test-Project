using System;

namespace Core
{
    public class LehmerRandom : FRandom
    {
        private const int A = 16807;
        private const int MBIG = 2147483647;
        private const int Q = 127773;
        private const int R = 2836;

        private int seed;

        public LehmerRandom(int seed)
        {
            SetSeed(seed);
        }

        public LehmerRandom()
        {
            long lSeed = DateTime.UtcNow.Ticks;
            int seed = (int)(lSeed & 0x7FFFFFFF);
            SetSeed(seed);
        }

        public void SetSeed(int seed)
        {
            if (seed <= 0 || seed >= MBIG)
            {
                throw new ArgumentException("Seed must >0 and <2147483647");
            }

            this.seed = seed;
        }

        public double Next()
        {
            int hi = seed / Q;
            int lo = seed % Q;
            seed = (A * lo) - (R * hi);
            if (seed <= 0)
            {
                seed = seed + MBIG;
            }

            return (seed * 1.0) / MBIG;
        }

        public int NextInt(int min, int max)
        {
            if (max < min)
            {
                throw new ArgumentException("Seed must >0 and <2147483647");
            }

            return (int)Math.Floor(Next() * (max - min) + min);
        }

        public bool NextBool(int probability, int factor)
        {
            if (probability > factor)
            {
                return true;
            }

            int val = NextInt(0, factor) + 1;
            return probability >= val;
        }
    }
}