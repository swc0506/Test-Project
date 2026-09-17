using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBulletRender : RenderObject
{
    private SkillBulletConfig mBulletCfg;
    public void SetRenderData(LogicObject logicObj, SkillBulletConfig bulletCfg)
    {
        SetLoigcObject(logicObj);
        mBulletCfg = bulletCfg;
    }

    public override void UpdatePosition()
    {
        base.UpdatePosition();

    }

    public override void UpdateDir()
    {
        transform.rotation = Quaternion.Euler(logicObject.LogicAngle.ToVector3());
    }
    public override void OnRelease()
    {
        base.OnRelease();
        GameObject.Destroy(gameObject);
    }
}
