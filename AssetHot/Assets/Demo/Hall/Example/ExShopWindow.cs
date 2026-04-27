using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZM.AssetFrameWork;

public class ExShopWindow : MonoBehaviour
{
    public Transform itemParent;
    // 商店道具列表
    private List<int> mItemIds = new List<int>();
    private List<ExShopItem> mExShopItems = new List<ExShopItem>();
    
    private void Awake()
    {
        AssetsFrame.HotAssets(BundleModuleEnum.GameItem, null, null, null);
    }
    
    private void Start()
    {
        for (int i = 0; i < 15; i++)
        {
            mItemIds.Add(i + 6001);
        }
        
        mExShopItems.Clear();
        //生成兑换道具列表
        foreach (var id in mItemIds)
        {
            GameObject itemObj = AssetsFrame.Instantiate(AssetsPathConfig.HALL_PREFAB_PATH + "ExShopItem", itemParent);
            itemObj.SetActive(true);
            ExShopItem exShopItem = itemObj.GetComponent<ExShopItem>();
            exShopItem.SetData(id);
            mExShopItems.Add(exShopItem);
        }
    }
    
    private void OnDestroy()
    {
        foreach (var exShopItem in mExShopItems)
        {
            exShopItem.Release();
        }
    }
}
