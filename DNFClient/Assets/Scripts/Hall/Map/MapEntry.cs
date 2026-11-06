using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapType
{ 
    None,//无配置
    Home,//家
    HeDunMaer,//赫顿马尔
    Dungeons,//地下城
}
/// <summary>
/// 门
/// </summary>
public enum DoorType
{
    None,//无配置
    Home,//家
    HeDunMaer,//赫顿马尔
    Dungeons,//地下城
}
public class MapEntry : MonoBehaviour
{
    /// <summary>
    /// 要去的地图
    /// </summary>
    public MapType mapType;
    /// <summary>
    /// 要去指定地图的哪一个门
    /// </summary>
    public DoorType doorType;
    /// <summary>
    /// 角色初始化位置
    /// </summary>
    public Transform roleInitPos;

}
