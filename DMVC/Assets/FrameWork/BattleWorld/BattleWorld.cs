using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GC.Battle
{
    public class BattleWorld : World
    {
        public override void OnCreat()
        {
            base.OnCreat();
            Debug.Log("BattleWorld OnCreat");
        }
        
        public override void OnDestroy()
        {
            base.OnDestroy();
            Debug.Log("BattleWorld OnDestroy");
        }

        public override void OnDestroyPostProcess(object args)
        {
            base.OnDestroyPostProcess(args);
            Debug.Log("BattleWorld OnDestroyPostProcess");
        }
    }
}