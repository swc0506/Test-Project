using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleWorldNodes : SingletonMono<BattleWorldNodes>
{
    public Transform[] heroRootArr;
    public Transform[] enemyRootArr;
    public Transform hudWindow;

    public Camera camera3D;
    public Camera uiCamera;
}
