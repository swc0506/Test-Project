/*----------------------------------------------------------------------------
* Title: #Title#
*
* Author: 铸梦
*
* Date: #CreateTime#
*
* Description:
*
* Remarks: QQ:975659933 邮箱：zhumengxyedu@163.com
*
* 教学网站：www.yxtown.com/user/38633b977fadc0db8e56483c8ee365a2cafbe96b
----------------------------------------------------------------------------*/

using My.Physics2D;
using UnityEngine;

public class WallGroup : MonoBehaviour
{
    public static WallGroup Instance;
    
    public Transform[] wallGroupTransArr;

    public FixIntBoxCollider2D[] wallGroupCollider2DArr;
    private void Awake()
    {
        Instance = this;
        wallGroupCollider2DArr = new FixIntBoxCollider2D[wallGroupTransArr.Length];
        for (int i = 0; i < wallGroupTransArr.Length; i++)
        {
            wallGroupCollider2DArr[i] = PhysicsWorld2D.Instance.GenerateFixIntBoxFormUnityCollider(wallGroupTransArr[i].GetComponent<BoxCollider2D>());
        }
    }
}
