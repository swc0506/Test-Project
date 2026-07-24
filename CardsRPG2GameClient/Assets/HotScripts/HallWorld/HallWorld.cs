using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.UI;

namespace ZMGC.Hall
{
    public class HallWorld : World
    {
        public override void OnCreate()
        {
            base.OnCreate();
            Debug.Log("HallWorld OnCreate");
            NetWorkManager.Instance.ConnectSocket();
            UIModule.Instance.PopUpWindow<LoginWindow>();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void OnDestroyPostProcess(object args)
        {
            base.OnDestroyPostProcess(args);
        }
    }
}