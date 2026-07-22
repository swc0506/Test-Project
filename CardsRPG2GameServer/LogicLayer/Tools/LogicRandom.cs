using System.Collections;
using System.Collections.Generic;
using System;

public class LogicRandom : Singleton<LogicRandom>
{
    public int seedId;//随机种子
    Random random;

    public void InitRandom(int seedId)
    {
        this.seedId = seedId;
        random = new Random(seedId);
    }

    public int Range(int min, int max)
    {
        return random.Next(min, max);
    }
}
