using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    /// <summary>
    /// 跟随目标
    /// </summary>
    public Transform target;

    /// <summary>
    /// 平滑时间
    /// </summary>
    public float smoothTime = 0.3f;

    /// <summary>
    /// 最小移动位置(通过读取地图配置进行赋值，如果测试代码可以通过手动或运行时赋值)
    /// </summary>
    public Vector2 minPosition;

    /// <summary>
    /// 最大移动位置(通过读取地图配置进行赋值，如果测试代码可以通过手动或运行时赋值)
    /// </summary>
    public Vector2 maxPosition;

    /// <summary>
    /// 人物偏离屏幕中心一定范围后开始跟随
    /// </summary>
    public float followDistance;

    //1.能够保证渲染完全完成后进行跟随 2.保证角色位置完全更新后进行跟随 3.能够防止摄像机抖动
    private void LateUpdate()
    {
        if (target != null)
        {
            //计算目标位置，使目标位置和摄像机位置保持在同一个z轴，用来计算摄像机需要移动的x轴的值
            Vector3 targetPos = new Vector3(target.position.x, transform.position.y, transform.position.z);
            //计算摄像机和角色之间的距离
            float distance = Vector2.Distance(targetPos, transform.position);
            //判断摄像机是否需要跟随
            bool isFollowTarget = distance > followDistance;
            //限制跟随目标的x位置，不能超过地图范围
            targetPos.x = Mathf.Clamp(targetPos.x, minPosition.x, maxPosition.x);
            if (isFollowTarget)
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, smoothTime * Time.deltaTime);
            }
        }
    }
}