using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LogicLayer
{
    public enum LogicObjectState
    {
        Survival, //存活中
        Dead, //死亡
        SurvivalWaiting, //存活等待中
    }

    public class LogicObject : LogicBehaviour
    {
        public LogicObjectState objectState = LogicObjectState.Survival;

        public void SetRenderObject(RenderObject renderObject)
        {
            objectState = LogicObjectState.Survival;
            RednerObj = renderObject;
            LogicPosition = new VInt3(renderObject.gameObject.transform.position);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
#if CLIENT_LOGIC
        RednerObj.OnRelease();
#endif
        }
    }
}