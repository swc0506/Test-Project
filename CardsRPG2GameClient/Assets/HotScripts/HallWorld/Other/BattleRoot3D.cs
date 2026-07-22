/*----------------------------------------------------------------------------
* Title: 帧同步定点数学库
*
* Author: 铸梦
*
* Date: 2025.02.20
*
* Description:基于定点数实现的一套AABB定点数学物理碰撞库，可用于客户端和服务端。
*
* Remarks: QQ:975659933 邮箱：zhumengxyedu@163.com
*
* 案例地址：www.yxtown.com/user/38633b977fadc0db8e56483c8ee365a2cafbe96b
----------------------------------------------------------------------------*/

using UnityEngine;
//using ZM.ZMAsset;

public class BattleRoot3D : MonoBehaviour
{
    public Camera battleCamera;
    public Transform cameraParent;
    public Transform seatRootTrans;
    public Transform[] leftSeatTransArr;
    public Transform[] rightSeatTransArr;
    public Transform pvpRootTrans;
    
    public Transform enemysConter;
    public Transform herosConter;
    public Transform conterTrans;
    
    private GameObject mMapObject;
    public GameObject[] mHeroSeatEffectArr;
    
    /// <summary>
    /// 加载指定地图
    /// </summary>
    /// <param name="mapName"></param>
    public void LoadMap(string mapName)
    {
        //mMapObject = ZMAsset.InstantiateObject(AssetsPathConfig.HALL_PREFABS_PATH + $"Battle/{mapName}", transform);
    }

    public void RevertCamera()
    {
        battleCamera.transform.SetParent(cameraParent);
        battleCamera.transform.localPosition = Vector3.zero;
        battleCamera.transform.localEulerAngles = Vector3.zero;
    }
    
    /// <summary>
    /// 设置座位特效状态
    /// </summary>
    /// <param name="state"></param>
    public void SetSeatEffectState(bool state)
    {
        foreach (var item in mHeroSeatEffectArr)
        {
            item.SetActive(state);
        }
    }

    public void OnRelease()
    {
        if (mMapObject!=null)
        {
            //ZMAsset.Release(mMapObject);
            mMapObject=null;;
        }
    }
}
