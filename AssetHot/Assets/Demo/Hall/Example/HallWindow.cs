using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZM.AssetFrameWork;

public class HallWindow : MonoBehaviour
{
    public Button exShopBtn;
    public Button exampleButton;
    
    // Start is called before the first frame update
    void Start()
    {
        exShopBtn.onClick.AddListener(OnClickExShop);
        exampleButton.onClick.AddListener(OnExampleButtonClick);
    }

    public void OnClickExShop()
    {
        AssetsFrame.Instantiate(AssetsPathConfig.HALL_PREFAB_PATH + "ExShopWindow", null);
    }
    
    public void OnExampleButtonClick()
    {
        AssetsFrame.Instantiate(AssetsPathConfig.HALL_PREFAB_PATH + "ExampleWindow", null);
    }
}
