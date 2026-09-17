using System.Collections;
using System.Collections.Generic;
using System;
using FixMath;

/// <summary>
/// 逻辑随机数生成器
/// </summary>
public class LogicRandom
{
    public int seedid;//随机种子
    Random random;//随机数生成器

    public LogicRandom(int seedid)
    {
        this.seedid = seedid;
        //局部的
        random = new Random(seedid);
    }
  
    public int Range(int min,int max)
    {
        return random.Next(min,max);
    }

    public FixInt Range(FixInt min, FixInt max)
    {
        return random.Next(min.IntValue, max.IntValue) / 1024f;
    }

    
}
