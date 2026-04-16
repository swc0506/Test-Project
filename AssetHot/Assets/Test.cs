using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.AssetFrameWork;

public class Test : MonoBehaviour
{
    private void Awake()
    {
        FrameBase.Instance.InitFrameWork();
    }

    void Start()
    {
        HotUpdateManager.Instance.CheckAssetsVersion(BundleModuleEnum.Game);
    }
}
