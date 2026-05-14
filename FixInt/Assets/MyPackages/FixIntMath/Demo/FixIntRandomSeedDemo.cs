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
using UnityEngine;
using ZM.FixIntMath;

public class FixIntRandomSeedDemo:MonoBehaviour
{
    private void Start()
    {
        //1.声明一个随机种子
        FixIntRandomSeed random = new FixIntRandomSeed(8734);
        //开始随机一个数，每个随机索引的随机数都是固定的，是根据随机种子生成的一套有序的随机数
        for (int i = 0; i < 10; i++)
        {
            Debug.Log($"index:{i} random:  {random.Range(0, 10000)}");
        }

        Debug.Log($"--------------------------------------------------------");
        //1.声明一个随机种子
        FixIntRandomSeed random2 = new FixIntRandomSeed(0);
        //开始随机一个数，每个随机索引的随机数都是固定的，是根据随机种子生成的一套有序的随机数
        for (int i = 0; i < 10; i++)
        {
            Debug.Log($"index:{i} random2:  {random2.Range(0, 10000)}");
        }
    }
}
