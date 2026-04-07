using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC.Hall
{
    public class HallWorld : World
    {
        public override void OnCreat()
        {
            base.OnCreat();
            Debug.Log("HallWorld OnCreat");
        }
        
        public override void OnDestroy()
        {
            base.OnDestroy();
            Debug.Log("HallWorld OnDestroy");
        }

        public override void OnDestroyPostProcess(object args)
        {
            base.OnDestroyPostProcess(args);
            Debug.Log("HallWorld OnDestroyPostProcess");
        }
    }
}