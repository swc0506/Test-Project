using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LogicLayer
{
    public interface ILogicBehaviour
    {
        void OnCreate();
        void OnLogicFrameUpdate();
        void OnDestroy();
    }
}