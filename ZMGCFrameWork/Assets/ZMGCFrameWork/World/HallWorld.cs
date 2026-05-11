using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZMGC.Hall
{
    public class HallWorld : World
    {
        public override void OnCretae()
        {
            base.OnCretae();
            Debug.Log("HallWorld  OnCretae>>>");
        }

        public static void EnterHallWorldFormGame()
        {
            UIManager.Instance.hallWindow.SetActive(true);
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            Debug.Log("HallWorld  OnDestroy>>>");
        }
        public override void OnDestroyPostProcess(object args)
        {
            base.OnDestroyPostProcess(args);
            Debug.Log("HallWorld  OnDestroyPostProcess>>>");

        }
    }
}