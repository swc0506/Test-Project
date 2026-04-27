using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZM.AssetFrameWork;

public class HallWindow : MonoBehaviour
{
    public Button exShopBtn;
    
    // Start is called before the first frame update
    void Start()
    {
        exShopBtn.onClick.AddListener(OnClickExShop);
    }

    public void OnClickExShop()
    {
        AssetsFrame.Instantiate(AssetsPathConfig.HALL_PREFAB_PATH + "ExShopWindow", null);
    }
}
