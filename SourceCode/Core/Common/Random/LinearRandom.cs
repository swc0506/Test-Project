using System;

namespace Core
{
    public class LinearRandom : FRandom
    {
        private const int MBIG = 2147483647;
        private const int MSEED = 161803398;
        private const int MZ = 0;
        private int inext;
        private int inextp;
        private int[] seedArray = new int[56];

        public LinearRandom(int seed)
        {
            SetSeed(seed);
        }

        public LinearRandom()
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

            int num1 = MSEED - (seed == int.MinValue ? int.MaxValue : Math.Abs(seed));
            this.seedArray[55] = num1;
            int num2 = 1;
            for (int index1 = 1; index1 < 55; ++index1)
            {
                int index2 = 21 * index1 % 55;
                this.seedArray[index2] = num2;
                num2 = num1 - num2;
                if (num2 < 0)
                {
                    num2 += int.MaxValue;
                }

                num1 = this.seedArray[index2];
            }

            for (int index3 = 1; index3 < 5; ++index3)
            {
                for (int index4 = 1; index4 < 56; ++index4)
                {
                    this.seedArray[index4] -= this.seedArray[1 + (index4 + 30) % 55];
                    if (this.seedArray[index4] < 0)
                        this.seedArray[index4] += int.MaxValue;
                }
            }

            this.inext = 0;
            this.inextp = 21;
        }

        public double Next()
        {
            return InternalSample() * 4.656612875245797E-10;
        }

        private int InternalSample()
        {
            int inext = this.inext;
            int inextp = this.inextp;
            int index1;
            if ((index1 = inext + 1) >= 56)
                index1 = 1;
            int index2;
            if ((index2 = inextp + 1) >= 56)
                index2 = 1;
            int num = this.seedArray[index1] - this.seedArray[index2];
            if (num == int.MaxValue)
            {
                --num;
            }

            if (num < 0)
            {
                num += int.MaxValue;
            }

            this.seedArray[index1] = num;
            this.inext = index1;
            this.inextp = index2;
            return num;
        }


        private double GetSampleForLargeRange()
        {
            int num = this.InternalSample();
            if (this.InternalSample() % 2 == 0)
                num = -num;
            return ((double)num + 2147483646.0) / 4294967293.0;
        }

        public int NextInt(int min, int max)
        {
            if (max < min)
            {
                throw new ArgumentException("Seed must >0 and <2147483647");
            }

            long num = (long)max - (long)min;
            return num <= (long)int.MaxValue
                ? (int)(this.Next() * (double)num) + min
                : (int)((long)(this.GetSampleForLargeRange() * (double)num) + (long)min);
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