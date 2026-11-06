using FixIntPhysics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    /// <summary>
    /// 地图类型
    /// </summary>
    public MapType mapType;
    /// <summary>
    /// 地图初始化位置
    /// </summary>
    public Vector3 mapInitPos;
   /// <summary>
   /// 最小移动位置(地图最小边界)
   /// </summary>
    public Vector2 minPosition;
    /// <summary>
    /// 最大移动位置(通过读取地图配置进行赋值，如果是测试代码可以通过手动或运行时赋值)
    /// </summary>
    public Vector2 maxPosition;
    /// <summary>
    /// 所有入口碰撞体
    /// </summary>
    public FixIntBoxColliderEventMono[] entryColliderArr;
    /// <summary>
    /// 地图入口配置
    /// </summary>
    public MapEntry[] mapEntry;

    /// <summary>
    /// 获取地图配置
    /// </summary>
    /// <param name="doorType"></param>
    /// <returns></returns>
    public MapEntry GetMapCfgData(DoorType doorType)
    {
        foreach (var item in mapEntry)
        {
            if ((int)item.doorType == (int)doorType)
            { 
                return item;
            }
        }
        return null;
    }

    /// <summary>
    /// 添加入口碰撞体事件
    /// </summary>
    /// <param name="collider"></param>
    public void AddEntryColliderEvent(FixIntBoxCollider collider)
    {
        foreach (var item in entryColliderArr)
        {
            item.AddCollider(collider);
        }
    }
    /// <summary>
    /// 移除碰撞体事件
    /// </summary>
    /// <param name="collider"></param>
    public void RemoveEntryColliderEvent(FixIntBoxCollider collider)
    {
        foreach (var item in entryColliderArr)
        {
            item.RemoveCollider(collider);
        }
    }

    public void ReleaseMapCollider()
    {
        foreach (var item in entryColliderArr)
            item.ReleaseCollider();
    }
}
