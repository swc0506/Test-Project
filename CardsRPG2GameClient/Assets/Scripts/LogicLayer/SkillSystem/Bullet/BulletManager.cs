using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : SingletonMono<BulletManager>, ILogicBehaviour
{
    public List<BulletLogic> bulletLogics = new List<BulletLogic>();

    public void OnCreate()
    {
    }

    public void OnDestroy()
    {
        for (int i = 0; i < bulletLogics.Count; i++)
        {
            bulletLogics[i].OnDestroy();
        }
        bulletLogics.Clear();
    }

    public void RemoveBullet(BulletLogic logic)
    {
        for (int i = 0; i < bulletLogics.Count; i++)
        {
            if (bulletLogics[i] == logic)
            {
                bulletLogics.RemoveAt(i);
                logic.OnDestroy();
            }
        }
    }

    /// <summary>
    /// 创建子弹
    /// </summary>
    /// <param name="bulletPfb"></param>
    /// <param name="attacker"></param>
    /// <param name="target"></param>
    /// <param name="fightTime"></param>
    /// <param name="onHitComplete"></param>
    public void CreateBullet(string bulletPfb, LogicObject attacker, LogicObject target, VInt fightTime,
        Action onHitComplete)
    {
        BulletLogic logic = new BulletLogic(attacker, target, fightTime, onHitComplete);
#if RENDER_LOGIC
        // 加载子弹预制体， 设置逻辑对象和渲染对象
        BulletRender bulletRender =
            ResourcesManager.Instance.LoadObject<BulletRender>(AssetPathConfig.SKILL_EFFECT + bulletPfb);
        bulletRender.transform.position = attacker.LogicPosition.vec3;
        logic.SetRenderObject(bulletRender);
        bulletRender.SetLogicObject(logic);
#endif
        logic.OnCreate();
        bulletLogics.Add(logic);
    }

    public void OnLogicFrameUpdate()
    {
        for (int i = bulletLogics.Count - 1; i >= 0; i--)
        {
            bulletLogics[i].OnLogicFrameUpdate();
        }
    }
}