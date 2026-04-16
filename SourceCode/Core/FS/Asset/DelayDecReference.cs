using UnityEngine;

namespace Core.FS
{
    public class DelayDecReference : IClearable
    {
        private static readonly ReferencePool refPool = new ReferencePool();

        public static ReferencePool RefPool
        {
            get { return refPool; }
        }

        public float DecRefTime { get; private set; }
        public int Count { get; private set; }

        public void SetDelayTime(float delayTime)
        {
            Count++;
            float time = Time.unscaledTime + delayTime;
            if (DecRefTime < time)
            {
                DecRefTime = time;
            }
        }

        public void Clear()
        {
            DecRefTime = 0;
            Count = 0;
        }
    }
}