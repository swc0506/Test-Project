using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZM.AssetFrameWork;

public class ButtonClick : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        AssetsFrame.Release(transform.parent.gameObject);
        AssetsFrame.ClearResourcesAssets(true);
        AssetsFrame.Instantiate("Assets/Demo/Hall/Prefab/HallWindow", null, Vector3.zero, Vector3.one,
            Quaternion.identity);
    }
}
