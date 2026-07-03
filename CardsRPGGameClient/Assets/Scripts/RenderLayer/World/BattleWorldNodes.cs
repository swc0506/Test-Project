using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleWorldNodes : SingletonMono<BattleWorldNodes>
{
    public Transform[] heroRootArr;
    public Transform[] enemyRootArr;
    public Transform hudWindow;
    public Transform heroCenter;
    public Transform enemyCenter;
    public Transform centerTrans;

    public Camera camera3D;
    public Camera uiCamera;
    
    public RoundWindow roundWindow;
    public BattleResultWindow battleResultWindow;
}
