using FixMath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroLogic : LogicActor
{
    /// <summary>
    /// 英雄id
    /// </summary>
    public int Heroid { get; private set; }

    public HeroLogic(int heroid,RenderObject renderObj)
    {
        Heroid = heroid;
        RenderObj = renderObj;
        ObjectType = LogicObjectType.Hero;
    }

    public override void OnLogicFrameUpdate()
    {
        base.OnLogicFrameUpdate();

    }
}
