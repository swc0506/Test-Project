using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UISetting", menuName = "UISetting", order = 0)]
public class UISetting : ScriptableObject
{
    private static UISetting instance;

    public static UISetting Instance { get { if (instance == null) { instance = Resources.Load<UISetting>("UISetting"); } return instance; } }

    /// <summary>
    /// 是否启用单遮模式
    /// </summary>
    public bool SINGMASK_SYSTRM;
}
