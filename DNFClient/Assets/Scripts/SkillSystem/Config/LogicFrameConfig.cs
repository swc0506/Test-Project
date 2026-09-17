using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicFrameConfig 
{
    //逻辑帧id 自增
    public static long LogicFrameId;
    //实际逻辑帧间隔
    public static float LogicFrameInterval = 0.066f;//一秒15帧
    //毫秒级逻辑帧间隔，用来计算当前逻辑帧累加时间
    public static int  LogicFrameIntervalms = 66;//一秒15帧
}
