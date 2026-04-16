using System;

namespace Core
{
    public class AtomicInt
    {
        private readonly int defValue;
        private int value;

        public AtomicInt() : this(0)
        {
        }

        public AtomicInt(int defValue)
        {
            this.value = this.defValue = defValue;
        }

        public int Get()
        {
            return value;
        }

        public int GetAndIncrement()
        {
            int val = value;
            value = value >= Int32.MaxValue ? defValue : ++value;
            return val;
        }

        public int IncrementAndGet()
        {
            value = value >= Int32.MaxValue ? defValue : ++value;
            return value;
        }
        
        public int GetAndDecrement()
        {
            int val = value;
            value = value <= Int32.MinValue ? defValue : --value;
            return val;
        }

        public int DecrementAndGet()
        {
            value = value <= Int32.MinValue ? defValue : --value;
            return value;
        }
       
    }
}